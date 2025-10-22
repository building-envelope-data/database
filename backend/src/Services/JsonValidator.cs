using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Utilities;
using Json.Schema;

namespace Database.Services;

public sealed class JsonValidator(
    IHttpClientFactory httpClientFactory
)
{
    private static readonly EvaluationOptions s_evaluationOptions =
        new()
        {
            ValidateAgainstMetaSchema = false,
            OutputFormat = OutputFormat.Hierarchical
        };

    public async Task<JsonSchema> LoadJsonSchemaAsync(
        Uri jsonSchemaLocator,
        CancellationToken cancellationToken
    )
    {
        using var httpClient = httpClientFactory.CreateClient();
        return await JsonSchema.FromStream(
            await httpClient.GetStreamAsync(jsonSchemaLocator, cancellationToken)
        );
    }

    public async Task<EvaluationResults> ValidateAsync(
        Uri jsonSchemaLocator,
        string jsonDataFilePath,
        CancellationToken cancellationToken
    )
    {
        using var fileStream = File.OpenRead(jsonDataFilePath);
        using var jsonDocument = await JsonDocument.ParseAsync(
            fileStream,
            JsonDocumentSettings.Lax,
            cancellationToken
        );
        return Validate(
            await LoadJsonSchemaAsync(jsonSchemaLocator, cancellationToken),
            jsonDocument.RootElement
        );
    }

    public EvaluationResults Validate(
        JsonSchema jsonSchema,
        JsonElement jsonElement
    )
    {
        s_evaluationOptions.SchemaRegistry.Fetch = JsonSchemaFetcher.FetchWithCaching;
        return jsonSchema.Evaluate(
            jsonElement,
            s_evaluationOptions
        );
    }
}