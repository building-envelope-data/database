using System;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Data.Filters;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;
using Microsoft.Extensions.Logging;
using HotChocolate.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Database.GraphQl.Filters;
using System.Net.Http;
using OpenIddict.Validation.AspNetCore;

namespace Database.Configuration
{
    public static class GraphQlConfiguration
    {
        public static class WellKnownSchemaNames
        {
            public const string Metabase = "metabase";
        }

        public static void ConfigureServices(
            IServiceCollection services,
            IWebHostEnvironment environment,
            AppSettings appSettings
            )
        {
            // Stitching Services
            // https://chillicream.com/docs/hotchocolate/v13/distributed-schema/schema-stitching
            var httpClientBuilder = services.AddHttpClient(
                WellKnownSchemaNames.Metabase,
                _ => _.BaseAddress = new Uri($"{appSettings.MetabaseHost}/graphql")
            );
            if (!environment.IsProduction())
            {
                httpClientBuilder.ConfigurePrimaryHttpMessageHandler(_ =>
                    new HttpClientHandler
                    {
                        // ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }
                );
            }
            // Automatic-Persisted-Queries Services
            services
            .AddMemoryCache()
            .AddSha256DocumentHashProvider(HotChocolate.Language.HashFormat.Hex);
            // GraphQL Server
            services
            .AddGraphQLServer()
            // Services https://chillicream.com/docs/hotchocolate/v13/integrations/entity-framework#registerdbcontext
            .RegisterDbContext<Data.ApplicationDbContext>(DbContextKind.Pooled)
            // Extensions
            .AddProjections()
            .AddFiltering<CustomFilterConvention>()
            .AddSorting()
            .AddAuthorization()
            .AddApolloTracing(
                HotChocolate.Execution.Options.TracingPreference.OnDemand
            ) // TODO Do we want or need this?
            .AddGlobalObjectIdentification()
            .AddQueryFieldToMutationPayloads()
            .ModifyOptions(options =>
              {
                  // https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Types/Configuration/Contracts/ISchemaOptions.cs
                  options.StrictValidation = true;
                  options.UseXmlDocumentation = false;
                  options.SortFieldsByName = true;
                  options.RemoveUnreachableTypes = false;
                  options.DefaultBindingBehavior = HotChocolate.Types.BindingBehavior.Implicit;
                  /* options.FieldMiddleware = ... */
              }
              )
            .ModifyRequestOptions(options =>
                {
                    // https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Execution/Options/RequestExecutorOptions.cs
                    /* options.ExecutionTimeout = ...; */
                    options.IncludeExceptionDetails = environment.IsDevelopment(); // Default is `Debugger.IsAttached`.
                    /* options.QueryCacheSize = ...; */
                    /* options.UseComplexityMultipliers = ...; */
                }
            )
            // TODO Configure `https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Validation/Options/ValidationOptions.cs`. But how?
            // Subscriptions
            /* .AddInMemorySubscriptions() */
            // TODO Persisted queries
            /* .AddFileSystemQueryStorage("./persisted_queries") */
            /* .UsePersistedQueryPipeline(); */
            .AddHttpRequestInterceptor(async (httpContext, requestExecutor, requestBuilder, cancellationToken) =>
            {
                // HotChocolate uses the default authentication scheme, which we
                // set to `null` in `AuthConfiguration` to force users to be
                // explicit about what scheme to use when making it easier to
                // grasp the various authentication flows.
                try
                {
                    await HttpContextAuthentication.Authenticate(httpContext);
                }
                catch (Exception e)
                {
                    // TODO Log to a `ILogger<GraphQlConfiguration>` instead.
                    Console.WriteLine(e);
                }
            })
            .AddDiagnosticEventListener(_ =>
                new GraphQl.LoggingDiagnosticEventListener(
                    _.GetApplicationService<ILogger<GraphQl.LoggingDiagnosticEventListener>>()
                )
            )
            // Scalar Types
            .AddType(new UuidType("Uuid", defaultFormat: 'D')) // https://chillicream.com/docs/hotchocolate/defining-a-schema/scalars#uuid-type
            .AddType(new UrlType("Url"))
            .AddType(new JsonType("Any", BindingBehavior.Implicit)) // https://chillicream.com/blog/2023/02/08/new-in-hot-chocolate-13#json-scalar
            .AddType(new GraphQl.LocaleType())
            // Query Types
            .AddQueryType(d => d.Name(nameof(GraphQl.Query)))
                .AddType<GraphQl.CalorimetricDataX.CalorimetricDataQueries>()
                .AddType<GraphQl.Databases.DatabaseQueries>()
                .AddType<GraphQl.DataX.DataQueries>()
                .AddType<GraphQl.GetHttpsResources.GetHttpsResourceQueries>()
                .AddType<GraphQl.HygrothermalDataX.HygrothermalDataQueries>()
                .AddType<GraphQl.OpticalDataX.OpticalDataQueries>()
                .AddType<GraphQl.PhotovoltaicDataX.PhotovoltaicDataQueries>()
                .AddType<GraphQl.Users.UserQueries>()
                .AddType<GraphQl.VerificationCodeQueries>()
            // Mutation Types
            .AddMutationType(d => d.Name(nameof(GraphQl.Mutation)))
                .AddType<GraphQl.CalorimetricDataX.CalorimetricDataMutations>()
                .AddType<GraphQl.Databases.DatabaseMutations>()
                .AddType<GraphQl.GetHttpsResources.GetHttpsResourceMutations>()
                .AddType<GraphQl.HygrothermalDataX.HygrothermalDataMutations>()
                .AddType<GraphQl.OpticalDataX.OpticalDataMutations>()
                .AddType<GraphQl.PhotovoltaicDataX.PhotovoltaicDataMutations>()
            /* .AddSubscriptionType(d => d.Name(nameof(GraphQl.Subscription))) */
            /*     .AddType<ComponentSubscriptions>() */
            // Object Types
            .AddType<GraphQl.Common.OpenEndedDateTimeRangeType>()
            .AddType<GraphQl.CalorimetricDataX.CalorimetricDataType>()
            .AddType<GraphQl.DataX.DataType>()
            .AddType<GraphQl.GetHttpsResources.GetHttpsResourceType>()
            .AddType<GraphQl.HygrothermalDataX.HygrothermalDataType>()
            .AddType<GraphQl.NamedMethodArgumentType>()
            .AddType<GraphQl.OpticalDataX.OpticalDataType>()
            .AddType<GraphQl.PhotovoltaicDataX.PhotovoltaicDataType>()
            .AddType<GraphQl.Users.UserType>()
            // Data Loaders
            /* .AddDataLoader<GraphQl.Components.ComponentByIdDataLoader>() */
            // Paging
            .SetPagingOptions(
                new PagingOptions
                {
                    MaxPageSize = int.MaxValue,
                    DefaultPageSize = int.MaxValue,
                    IncludeTotalCount = true,
                    // TODO I actually want to infer connection names from fields (which is the default in HotChocolate). However, the current `database.graphql` schema that I hand-wrote still infers connection names from types.
                    InferConnectionNameFromField = false
                }
            )
            // Stitching
            // .AddRemoteSchema(WellKnownSchemaNames.Metabase, ignoreRootTypes: true)
            // .AddTypeExtensionsFromString(@"
            //     extend type OpticalData {
            //         component: Component @delegate(schema: 'Metabase', path: 'component(uuid: $fields:ComponentId)')
            //     }
            // ")
            // Automatic Peristed Queries
            .UseAutomaticPersistedQueryPipeline()
            .AddInMemoryQueryStorage();
        }
    }

    // See https://chillicream.com/docs/hotchocolate/fetching-data/filtering/#filter-conventions
    public class CustomFilterConvention : FilterConvention
    {
        protected override void Configure(IFilterConventionDescriptor descriptor)
        {
            descriptor.AddDefaults();
            // Use argument name `where`
            descriptor.ArgumentName("where");
            // Allow conjunction and disjunction
            descriptor.AllowAnd();
            descriptor.AllowOr();
            // Bind custom types
            descriptor.BindRuntimeType<Data.GetHttpsResource, GraphQl.GetHttpsResources.GetHttpsResourceFilterType>();
            descriptor.BindRuntimeType<Data.NamedMethodArgument, GraphQl.NamedMethodArgumentFilterType>();
            descriptor.BindRuntimeType<Data.CalorimetricData, GraphQl.CalorimetricDataX.CalorimetricDataFilterType>();
            descriptor.BindRuntimeType<Data.IData, GraphQl.DataX.DataFilterType>();
            descriptor.BindRuntimeType<Data.HygrothermalData, GraphQl.HygrothermalDataX.HygrothermalDataFilterType>();
            descriptor.BindRuntimeType<Data.OpticalData, GraphQl.OpticalDataX.OpticalDataFilterType>();
            descriptor.BindRuntimeType<Data.PhotovoltaicData, GraphQl.PhotovoltaicDataX.PhotovoltaicDataFilterType>();
        }

        // TODO Overriding and changing type names in this way is _super_ error-prone. However, using `descriptor.Configure<...FilterInputType<T>>(x => x.Name(name))` does not work. Why?
        // For the base implementation see https://github.com/ChilliCream/hotchocolate/blob/f0dff93a14cb7ddecc7b3a0530a687a5bc4bad71/src/HotChocolate/Data/src/Data/Filters/Convention/FilterConvention.cs#L129
        public override string GetTypeName(Type runtimeType)
        {
            var nameString = base.GetTypeName(runtimeType);
            return
                new Regex(@"IData").Replace(
                    new Regex(@"Double").Replace(
                        new Regex(@"Float").Replace(
                            new Regex(@"ListString").Replace(
                                new Regex(@"ListFloat").Replace(
                                    new Regex(@"Guid").Replace(
                                        new Regex(@"^Comparable").Replace(
                                            new Regex(@"FilterInput$").Replace(
                                                new Regex(@"OperationFilterInput").Replace(
                                                    nameString,
                                                    "PropositionInput"
                                                ),
                                                "PropositionInput"
                                            ),
                                            ""
                                        ),
                                        "Uuid"
                                    ),
                                    "Floats"
                                ),
                                "Strings"
                            ),
                            "Float"
                        ),
                        "Float"
                    ),
                    "Data"
                );
        }
    }

    public static class FilterConventionDescriptorExtensions
    {
        // Inspired by FilterConventionDescriptorExtensions#AddDefaults
        // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L16
        public static IFilterConventionDescriptor AddDefaults(
            this IFilterConventionDescriptor descriptor) =>
            descriptor.AddDefaultOperations().BindDefaultTypes().UseQueryableProvider();

        // Inspired by FilterConventionDescriptorExtensions#AddDefaultOperations
        // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L28
        public static IFilterConventionDescriptor AddDefaultOperations(
            this IFilterConventionDescriptor descriptor)
        {
            descriptor.Operation(DefaultFilterOperations.Equals).Name("equalTo");
            descriptor.Operation(DefaultFilterOperations.NotEquals).Name("notEqualTo");
            descriptor.Operation(DefaultFilterOperations.GreaterThan).Name("greaterThan");
            descriptor.Operation(DefaultFilterOperations.NotGreaterThan).Name("notGreaterThan");
            descriptor.Operation(DefaultFilterOperations.GreaterThanOrEquals).Name("greaterThanOrEqualTo");
            descriptor.Operation(DefaultFilterOperations.NotGreaterThanOrEquals).Name("notGreaterThanOrEqualTo");
            descriptor.Operation(DefaultFilterOperations.LowerThan).Name("lessThan");
            descriptor.Operation(DefaultFilterOperations.NotLowerThan).Name("lessThan");
            descriptor.Operation(DefaultFilterOperations.LowerThanOrEquals).Name("notLessThanOrEqualTo");
            descriptor.Operation(DefaultFilterOperations.NotLowerThanOrEquals).Name("notLessThanOrEqualTo");
            descriptor.Operation(DefaultFilterOperations.Contains).Name("contains");
            descriptor.Operation(DefaultFilterOperations.NotContains).Name("doesNotContain");
            descriptor.Operation(DefaultFilterOperations.In).Name("in");
            descriptor.Operation(DefaultFilterOperations.NotIn).Name("notIn");
            descriptor.Operation(DefaultFilterOperations.StartsWith).Name("startsWith");
            descriptor.Operation(DefaultFilterOperations.NotStartsWith).Name("doesNotStartWith");
            descriptor.Operation(DefaultFilterOperations.EndsWith).Name("endsWith");
            descriptor.Operation(DefaultFilterOperations.NotEndsWith).Name("doesNotEndWith");
            descriptor.Operation(DefaultFilterOperations.All).Name("all");
            descriptor.Operation(DefaultFilterOperations.None).Name("none");
            descriptor.Operation(DefaultFilterOperations.Some).Name("some");
            descriptor.Operation(DefaultFilterOperations.Any).Name("any");
            descriptor.Operation(DefaultFilterOperations.And).Name("and");
            descriptor.Operation(DefaultFilterOperations.Or).Name("or");
            descriptor.Operation(DefaultFilterOperations.Data).Name("data");
            descriptor.Operation(AdditionalFilterOperations.Not).Name("not");
            // TODO `inClosedInterval`
            return descriptor;
        }

        // Inspired by FilterConventionDescriptorExtensions#BindDefaultTypes
        // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L73
        public static IFilterConventionDescriptor BindDefaultTypes(
            this IFilterConventionDescriptor descriptor)
        {
            descriptor
                .BindRuntimeType<string, StringOperationFilterInputType>()
                .BindRuntimeType<bool, BooleanOperationFilterInputType>()
                .BindRuntimeType<bool?, BooleanOperationFilterInputType>()
                .BindComparableType<byte>("BytePropositionInput")
                .BindComparableType<short>("ShortPropositionInput")
                .BindComparableType<int>("IntPropositionInput")
                .BindComparableType<long>("LongPropositionInput")
                .BindComparableType<float>("FloatXPropositionInput")
                .BindComparableType<double>("FloatPropositionInput")
                .BindComparableType<decimal>("DecimalPropositionInput")
                .BindComparableType<sbyte>("SignedBytePropositionInput")
                .BindComparableType<ushort>("UnsignedShortPropositionInput")
                .BindComparableType<uint>("UnsignedIntPropositionInput")
                .BindComparableType<ulong>("UnsigendLongPropositionInput")
                .BindComparableType<Guid>("UuidPropositionInput")
                .BindComparableType<DateTime>("DateTimePropositionInput")
                .BindComparableType<DateTimeOffset>("DateTimeOffsetPropositionInput")
                .BindComparableType<TimeSpan>("TimeSpanPropositionInput");
            // TODO Why does this not work?
            // descriptor
            //     .Configure<StringOperationFilterInputType>(x => x.Name("StringPropositionInput"))
            //     .Configure<BooleanOperationFilterInputType>(x => x.Name("BooleanPropositionInput"));
            return descriptor;
        }

        // Inspired by FilterConventionDescriptorExtensions#FilterConventionDescriptorExtensions
        // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L102
        private static IFilterConventionDescriptor BindComparableType<T>(
            this IFilterConventionDescriptor descriptor,
            string? name = null)
            where T : struct
        {
            descriptor
                .BindRuntimeType<T, ComparableOperationFilterInputType<T>>()
                .BindRuntimeType<T?, ComparableOperationFilterInputType<T?>>();
            // TODO Why does this not work?
            // if (name is not null)
            // {
            //     descriptor
            //         .Configure<ComparableOperationFilterInputType<T>>(x => x.Name(name))
            //         .Configure<ComparableOperationFilterInputType<T?>>(x => x.Name($"Maybe{name}"));
            // }
            return descriptor;
        }
    }
}