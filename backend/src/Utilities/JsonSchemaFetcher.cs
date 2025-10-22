using Json.Schema;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Utilities;

public static class JsonSchemaFetcher
{
    private static readonly MemoryCache s_schemaCache = new(new MemoryCacheOptions());
    private static readonly SemaphoreSlim s_lock = new(1, 1);

    public static JsonSchema? FetchWithCaching(Uri locator)
    {
        var task = FetchWithCachingAsync(locator);
        task.Wait();
        return task.Result;
    }

    public static async Task<JsonSchema?> FetchWithCachingAsync(Uri locator)
    {
        // Lock to prevent multiple concurrent downloads for the same URI
        await s_lock.WaitAsync();
        try
        {
            // Check if the schema is already in the cache
            if (s_schemaCache.Get(locator.AbsoluteUri) is JsonSchema cachedSchema)
            {
                return cachedSchema;
            }
            // Download the schema if it's not in the cache
            using var client = new HttpClient();
            using var stream = await client.GetStreamAsync(locator);
            var downloadedSchema = await JsonSchema.FromStream(stream);
            // Add the downloaded schema to the cache
            s_schemaCache.Set(locator.AbsoluteUri, downloadedSchema, DateTimeOffset.Now.AddDays(1));
            return downloadedSchema;
        }
        catch (Exception exception)
        {
            // Console.WriteLine($"Error fetching schema from {locator}: {exception.Message}");
            return null;
        }
        finally
        {
            s_lock.Release();
        }
    }
}