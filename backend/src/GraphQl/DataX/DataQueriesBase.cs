using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.GraphQl.Scalars;
using Database.Services;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

public abstract class DataQueriesBase<TData>
    where TData : class, IData
{
    protected static Task<HotChocolate.Types.Pagination.Connection<TData>> GetAllDataAsync(
        Func<ApplicationDbContext, IQueryable<TData>> getData,
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return accessPolicyService.ApplyAsync(
            databaseContext => getData(databaseContext).AsNoTracking()
                .Where(_ => _.PublishingState != Enumerations.PublishingState.PENDING)
                .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder),
            async policedData =>
            {
                var connection = await policedData
                    .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                    .ToConnectionAsync();
                var nodes = connection.Edges.Select(_ => _.Node).ToList().AsReadOnly();
                return (nodes, connection);
            },
            databaseContextFactory,
            cancellationToken
        );
    }

    protected static async Task<HotChocolate.Types.Pagination.Connection<TData>> GetAllPendingDataAsync(
        Func<ApplicationDbContext, IQueryable<TData>> getData,
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return await Enumerable.Empty<TData>()
                .AsQueryable()
                .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                .ToConnectionAsync();
        }
        return await accessPolicyService.ApplyAsync(
            databaseContext => getData(databaseContext).AsNoTracking()
                .Where(_ => _.PublishingState == Enumerations.PublishingState.PENDING)
                .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder),
            async policedData =>
            {
                var connection = await policedData
                    .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                    .ToConnectionAsync();
                var nodes = connection.Edges.Select(_ => _.Node).ToList().AsReadOnly();
                return (nodes, connection);
            },
            databaseContextFactory,
            cancellationToken
        );
    }

    protected static Task<bool> HasDataAsync(
        Func<ApplicationDbContext, IQueryable<TData>> getData,
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return accessPolicyService.ApplyAsync(
            databaseContext => getData(databaseContext).AsNoTracking()
                .Where(_ => _.PublishingState != Enumerations.PublishingState.PENDING)
                .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder),
            async policedData =>
            {
                var nodes = (await policedData.ToListAsync(cancellationToken)).AsReadOnly();
                return (nodes, nodes.Count > 0);
            },
            databaseContextFactory,
            cancellationToken
        );
    }

    internal static async Task<TData?> GetDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        Func<ApplicationDbContext, IQueryable<TData>> getData,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        ApplicationDbContext databaseContext,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        var exists = await getData(databaseContext).AsNoTracking()
            .Where(_ => _.Id == id)
            .AnyAsync(cancellationToken);
        return await accessPolicyService.ApplyAsync<TData, TData?>(
            databaseContext => getData(databaseContext).AsNoTracking()
                .Where(_ => _.Id == id),
            async policedData =>
            {
                var node = await policedData.SingleOrDefaultAsync(cancellationToken);
                if (exists && node is null)
                {
                    authorization.ReportUnauthorizedError(resolverContext);
                }
                return (node is null ? [] : [node], node);
            },
            databaseContextFactory,
            cancellationToken
        );
    }
}