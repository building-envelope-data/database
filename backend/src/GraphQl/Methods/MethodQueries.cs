using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Json;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Database.GraphQl.Methods;

[SuppressMessage("Naming", "CA1707")]
public enum CalculateMethodErrorCode
{
    UNKNOWN_METHOD,
    UNKNOWN_DATABASE,
    UNKNOWN_DATA,
    UNSUPPORTED_DATA_FORMAT,
    HASH_VALUE_CALCULATION_FAILED,
    HASH_VALUE_MISMATHCH
}

public sealed record CalculateMethodError(
    CalculateMethodErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CalculateMethodErrorCode>(Code, Message, Path);

public sealed record CalculateMethodPayload(
    JsonElement? Result,
    IReadOnlyCollection<CalculateMethodError>? Errors
) : Payload;

[ExtendObjectType(nameof(Query))]
public sealed class MethodQueries
{
    public async Task<CalculateMethodPayload> CalculateMethodWithDataUploadAsync(
        Guid methodId,
        [GraphQLType<NonNullType<UploadType>>] IFile data,
        MethodFactory methodFactory,
        CancellationToken cancellationToken
    )
    {
        var method = methodFactory.GetMethod(methodId);
        if (method is null)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(methodId)]
                )]
            );
        }
        using var stream = data.OpenReadStream();
        using var jsonData = await JsonDocument.ParseAsync(
            stream,
            JsonDocumentSettings.Lax,
            cancellationToken
        );
        var result = method.Calculate(jsonData.RootElement);
        return new CalculateMethodPayload(result, null);
    }

    public async Task<CalculateMethodPayload> CalculateMethodAsync(
        Guid methodId,
        CrossDatabaseDataReferenceInput dataReference,
        AppSettings appSettings,
        MethodFactory methodFactory,
        ApiRequestService apiRequestService,
        IDataByDatabaseAndIdAndKindDataLoader dataByDatabaseAndIdAndKind,
        QueryData queryData,
        IResolverContext resolverContext,
        UrlEncoder urlEncoder,
        CancellationToken cancellationToken
    )
    {
        var method = methodFactory.GetMethod(methodId);
        if (method is null)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(methodId)]
                )]
            );
        }
        // We fetch the data through the metabase because, if possible, it adds
        // sanitized authorization headers.
        var database = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => dataByDatabaseAndIdAndKind.LoadAsync(
                (dataReference.DatabaseId, dataReference.DataId, dataReference.DataKind),
                cancellationToken
            ),
            resolverContext,
            DatabaseDataLoader.GetGraphQlEndpoint(appSettings)
        );
        if (database is null)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_DATABASE,
                    $"The database is unknown.",
                    [nameof(dataReference), nameof(dataReference.DatabaseId).ToLowerFirst()]
                )]
            );
        }
        if (database.Data is null)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_DATA,
                    $"The data is unknown.",
                    [nameof(dataReference), nameof(dataReference.DataId).ToLowerFirst()]
                )]
            );
        }
        // TODO Support non-JSON data formats.
        var dataFormat = database.Data.ResourceTree.Root.Value.DataFormat;
        if (dataFormat.MediaType != MediaTypeNames.Application.Json)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNSUPPORTED_DATA_FORMAT,
                    $"The data format {dataFormat.Id} of the root resource is unsupported because its media type is not '{MediaTypeNames.Application.Json}' but '{dataFormat.MediaType}'.",
                    [nameof(dataReference)]
                )]
            );
        }
        // We route to `response.Data.Data.ResourceTree.Root.Value.Locator`
        // through the metabase because, if possible, it adds sanitized
        // authorization headers.
        var locator = appSettings.MetabaseGetHttpsResourceEndpoint(database.Data.ResourceTree.Root.VertexId, dataReference.ToDomainModel(), urlEncoder);
        var (resourceContent, sha256HashValue) = await apiRequestService.PerformHttpGetRequest(
            locator, cancellationToken
        );
        if (sha256HashValue is null)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.HASH_VALUE_CALCULATION_FAILED,
                    $"Could not calculate the SHA256 hash value of the root resource, which is needed to make sure that the data was not tempered with.",
                    [nameof(dataReference)]
                )]
            );
        }
        if (sha256HashValue != database.Data.ResourceTree.Root.Value.HashValue)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.HASH_VALUE_MISMATHCH,
                    $"The SHA256 hash value '{sha256HashValue}' of the received root-resource content is different from the expected value '{database.Data.ResourceTree.Root.Value.HashValue}'.",
                    [nameof(dataReference)]
                )]
            );
        }
        var result = method.Calculate(resourceContent);
        return new CalculateMethodPayload(result, null);
    }
}