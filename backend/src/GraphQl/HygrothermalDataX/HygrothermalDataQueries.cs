using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Guid = System.Guid;

namespace Database.GraphQl.HygrothermalDataX;

[ExtendObjectType(nameof(Query))]
public sealed class HygrothermalDataQueries
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering]
    [UseSorting]
    public IQueryable<HygrothermalData> GetAllHygrothermalData(
        DateTime? timestamp,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context
    )
    {
        // TODO Use `timestamp` and `locale`.
        return context.HygrothermalData;
    }

    public Task<HygrothermalData?> GetHygrothermalDataAsync(
        Guid id,
        DateTime? timestamp,
        [GraphQLType<LocaleType>] string? locale,
        HygrothermalDataByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `timestamp` and `locale`.
        return byId.LoadAsync(
            id,
            cancellationToken
        );
    }
}