using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

namespace Database.GraphQl.GetHttpsResources
{
    [ExtendObjectType(nameof(Mutation))]
    public sealed class GetHttpsResourceMutations
    {
        [UseDbContext(typeof(Data.ApplicationDbContext))]
        // [UseUserManager]
        [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
        public async Task<CreateGetHttpsResourcePayload> CreateGetHttpsResourceAsync(
            CreateGetHttpsResourceInput input,
            // [GlobalState(nameof(ClaimsPrincipal))] ClaimsPrincipal claimsPrincipal,
            // [ScopedService] UserManager<Data.User> userManager,
            [ScopedService] Data.ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            // if (!await GetHttpsResourceAuthorization.IsAuthorizedToCreateGetHttpsResourceForInstitution(
            //      claimsPrincipal,
            //      input.CreatorId,
            //      userManager,
            //      context,
            //      cancellationToken
            //      ).ConfigureAwait(false)
            // )
            // {
            //     return new CreateGetHttpsResourcePayload(
            //         new CreateGetHttpsResourceError(
            //           CreateGetHttpsResourceErrorCode.UNAUTHORIZED,
            //           "You are not authorized to create optical data for the institution.",
            //           new[] { nameof(input), nameof(input.CreatorId).FirstCharToLower() }
            //         )
            //     );
            // }
            var getHttpsResource = new Data.GetHttpsResource(
                description: input.Description,
                hashValue: input.HashValue,
                locator: input.Locator,
                formatId: input.FormatId,
                dataId: input.DataId,
                parentId: input.ParentId,
                archivedFilesMetaInformation: input.ArchivedFilesMetaInformation.Select(i =>
                    new Data.FileMetaInformation(
                        path: i.Path,
                        formatId: i.FormatId
                    )
                ).ToList(),
                appliedConversionMethod:
                    input.AppliedConversionMethod is null
                    ? null
                    : new Data.ToTreeVertexAppliedConversionMethod(
                        methodId: input.AppliedConversionMethod.MethodId,
                        arguments: input.AppliedConversionMethod.Arguments.Select(a =>
                            new Data.NamedMethodArgument(
                                name: a.Name,
                                // TODO Turn `a.Value` into `JsonDocument`. It comes
                                // as nested `IReadOnlyDictionary/-List` as said on
                                // https://chillicream.com/docs/hotchocolate/v11/defining-a-schema/scalars/#any-type
                                // Take inspiration from
                                // https://josef.codes/custom-dictionary-string-object-jsonconverter-for-system-text-json/
                                // and
                                // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectJsonConverter/blob/develop/src/JOS.SystemTextJsonDictionaryObjectModelBinder/DictionaryStringObjectJsonConverter.cs
                                // This is also needed in `OpticalDataMutations`.
                                value: JsonDocument.Parse(@"""TODO""")
                            )
                        ).ToList(),
                        sourceName: input.AppliedConversionMethod.SourceName
                    )
            );
            context.GetHttpsResources.Add(getHttpsResource);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return new CreateGetHttpsResourcePayload(getHttpsResource);
        }
    }
}