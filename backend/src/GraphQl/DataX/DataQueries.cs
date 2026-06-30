using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

[ExtendObjectType(nameof(Query))]
public sealed class DataQueries
{
    public Task<IData?> GetDataAsync(
        Guid id,
        DataKind dataKind,
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        ApplicationDbContext databaseContext,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return DataQueriesBase<IData>.GetDataAsync(
            id,
            locale,
            databaseContext => databaseContext.Data(dataKind),
            databaseContextFactory,
            accessPolicyService,
            databaseContext,
            resolverContext,
            authorization,
            cancellationToken
        );
    }
}