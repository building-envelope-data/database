using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Extensions;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

namespace Database.GraphQl.OpticalDataX
{
    [ExtendObjectType(nameof(Mutation))]
    public sealed class OpticalDataMutations
    {
        // [UseUserManager]
        // [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
        public async Task<CreateOpticalDataPayload> CreateOpticalDataAsync(
            CreateOpticalDataInput input,
            // ClaimsPrincipal claimsPrincipal,
            // [ScopedService] UserManager<Data.User> userManager,
            Data.ApplicationDbContext context,
            [Service] AppSettings appSettings,
            CancellationToken cancellationToken
        )
        {
            // if (!await OpticalDataAuthorization.IsAuthorizedToCreateOpticalDataForInstitution(
            //      claimsPrincipal,
            //      input.CreatorId,
            //      userManager,
            //      context,
            //      cancellationToken
            //      ).ConfigureAwait(false)
            // )
            if (input.AccessToken != appSettings.AccessToken)
            {
                return new CreateOpticalDataPayload(
                    new CreateOpticalDataError(
                      CreateOpticalDataErrorCode.UNAUTHORIZED,
                      $"The access token {input.AccessToken} is invalid.",
                      new[] { nameof(input), nameof(input.AccessToken).FirstCharToLower() }
                    )
                );
            }
            var opticalData = new Data.OpticalData(
                locale: input.Locale,
                componentId: input.ComponentId,
                name: input.Name,
                description: input.Description,
                warnings: input.Warnings,
                creatorId: input.CreatorId,
                createdAt: input.CreatedAt,
                appliedMethod: new Data.AppliedMethod(
                    methodId: input.AppliedMethod.MethodId,
                    arguments: input.AppliedMethod.Arguments
                        .Select(a => new Data.NamedMethodArgument(
                            name: a.Name,
                            // TODO Turn `a.Value` into `JsonDocument`. It comes
                            // as nested `IReadOnlyDictionary/-List` as said on
                            // https://chillicream.com/docs/hotchocolate/v11/defining-a-schema/scalars/#any-type
                            // Take inspiration from
                            // https://josef.codes/custom-dictionary-string-object-jsonconverter-for-system-text-json/
                            // and
                            // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectJsonConverter/blob/develop/src/JOS.SystemTextJsonDictionaryObjectModelBinder/DictionaryStringObjectJsonConverter.cs
                            // This is also needed in `GetHttpsResourceMutations`.
                            value: JsonDocument.Parse(@"""TODO""")
                        ))
                        .ToList(),
                    sources: input.AppliedMethod.Sources
                        .Select(s => new Data.NamedMethodSource(
                            name: s.Name,
                            value: new Data.CrossDatabaseDataReference(
                                dataId: s.Value.DataId,
                                dataTimestamp: s.Value.DataTimestamp,
                                dataKind: s.Value.DataKind,
                                databaseId: s.Value.DatabaseId
                            )
                        ))
                        .ToList()
                ),
                approvals: input.Approvals.Select(a =>
                    new Data.DataApproval(
                        timestamp: a.Timestamp,
                        signature: a.Signature,
                        keyFingerprint: a.KeyFingerprint,
                        query: a.Query,
                        response: a.Response,
                        approverId: a.ApproverId,
                        publication : a.Publication is null ? null :
                                new Data.Publication(
                                title: a.Publication.Title,
                                @abstract: a.Publication.Abstract,
                                section: a.Publication.Section,
                                authors: a.Publication.Authors,
                                doi: a.Publication.Doi,
                                arXiv: a.Publication.ArXiv,
                                urn: a.Publication.Urn,
                                webAddress: a.Publication.WebAddress
                        ),
                        standard : a.Standard is null ? null :
                                new Data.Standard(
                                title: a.Standard.Title,
                                @abstract: a.Standard.Abstract,
                                section: a.Standard.Section,
                                year: a.Standard.Year,
                                standardizers: a.Standard.Standardizers,
                                locator: a.Standard.Locator
                        )
                    )
                ).ToList(),
                // approval: input.Approval
                nearnormalHemisphericalVisibleTransmittances: input.NearnormalHemisphericalVisibleTransmittances,
                nearnormalHemisphericalVisibleReflectances: input.NearnormalHemisphericalVisibleReflectances,
                nearnormalHemisphericalSolarTransmittances: input.NearnormalHemisphericalSolarTransmittances,
                nearnormalHemisphericalSolarReflectances: input.NearnormalHemisphericalSolarReflectances,
                infraredEmittances: input.InfraredEmittances,
                colorRenderingIndices: input.ColorRenderingIndices,
                cielabColors: input.CielabColors.Select(c =>
                    new Data.CielabColor(
                        lStar: c.LStar,
                        aStar: c.AStar,
                        bStar: c.BStar
                    )
                ).ToList()
            );
            var resource = new Data.GetHttpsResource(
                    description: input.RootResource.Description,
                    hashValue: input.RootResource.HashValue,
                    dataFormatId: input.RootResource.DataFormatId,
                    parentId: null,
                    archivedFilesMetaInformation: input.RootResource.ArchivedFilesMetaInformation.Select(i =>
                        new Data.FileMetaInformation(
                            path: i.Path,
                            dataFormatId: i.DataFormatId
                        )
                    ).ToList(),
                    appliedConversionMethod:
                        input.RootResource.AppliedConversionMethod is null
                        ? null
                        : new Data.ToTreeVertexAppliedConversionMethod(
                            methodId: input.RootResource.AppliedConversionMethod.MethodId,
                            arguments: input.RootResource.AppliedConversionMethod.Arguments.Select(a =>
                                new Data.NamedMethodArgument(
                                    name: a.Name,
                                    // TODO Turn `a.Value` into `JsonDocument`. It comes
                                    // as nested `IReadOnlyDictionary/-List` as said on
                                    // https://chillicream.com/docs/hotchocolate/v11/defining-a-schema/scalars/#any-type
                                    // Take inspiration from
                                    // https://josef.codes/custom-dictionary-string-object-jsonconverter-for-system-text-json/
                                    // and
                                    // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectJsonConverter/blob/develop/src/JOS.SystemTextJsonDictionaryObjectModelBinder/DictionaryStringObjectJsonConverter.cs
                                    // This is also needed in `GetHttpsResourceMutations`.
                                    value: JsonDocument.Parse(@"""TODO""")
                                )
                            ).ToList(),
                            sourceName: input.RootResource.AppliedConversionMethod.SourceName
                        )
            );
            opticalData.Resources.Add(resource);
            context.OpticalData.Add(opticalData);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return new CreateOpticalDataPayload(opticalData);
        }
    }
}
