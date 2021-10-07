using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

namespace Database.GraphQl.OpticalDataX
{
    [ExtendObjectType(nameof(Mutation))]
    public sealed class OpticalDataMutations
    {
        [UseDbContext(typeof(Data.ApplicationDbContext))]
        // [UseUserManager]
        [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
        public async Task<CreateOpticalDataPayload> CreateOpticalDataAsync(
            CreateOpticalDataInput input,
            // [GlobalState(nameof(ClaimsPrincipal))] ClaimsPrincipal claimsPrincipal,
            // [ScopedService] UserManager<Data.User> userManager,
            [ScopedService] Data.ApplicationDbContext context,
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
            // {
            //     return new CreateOpticalDataPayload(
            //         new CreateOpticalDataError(
            //           CreateOpticalDataErrorCode.UNAUTHORIZED,
            //           "You are not authorized to create optical data for the institution.",
            //           new[] { nameof(input), nameof(input.CreatorId).FirstCharToLower() }
            //         )
            //     );
            // }
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
                        approverId: a.ApproverId
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
            context.OpticalData.Add(opticalData);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return new CreateOpticalDataPayload(opticalData);
        }
    }
}