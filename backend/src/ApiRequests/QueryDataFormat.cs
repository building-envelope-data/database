using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Database.Services;

namespace Database.ApiRequests;

public sealed class QueryDataFormat
{
    private const string QueryFileName = "DataFormat.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record DataFormat(
        Guid Uuid,
        string Name,
        string? Extension,
        string Description,
        string MediaType,
        Uri? SchemaLocator
    // DataFormatManagerEdge manager,
    // Reference reference
    );

    private sealed record DataFormatData(DataFormat? DataFormat);

    public static async Task<DataFormat?> Do(
        Guid dataFormatId,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        CancellationToken cancellationToken
    )
    {
        return (await apiRequestService.QueryGraphQl<DataFormatData>(
            GetGraphQlEndpoint(appSettings),
            new GraphQLRequest(
                await apiRequestService.ConstructGraphQlQuery(QueryFileName),
                new
                {
                    id = dataFormatId
                },
                "DataFormat"
            ),
            cancellationToken
        )).Data.DataFormat;
    }
}