using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.GraphQl.DataX;
using Database.GraphQl.Extensions;
using Database.GraphQl.Entities;
using GreenDonut;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class DataAccessPolicyType
    : EntityType<DataAccessPolicy, DataAccessPolicyByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(_ => _.GetData(default!))
            .Ignore();
        descriptor
            .Field(_ => _.GetDataId(default!))
            .Ignore();
        descriptor
            .Field(_ => _.CalorimetricDataId)
            .Ignore();
        descriptor
            .Field(_ => _.CalorimetricData)
            .Ignore();
        descriptor
            .Field(_ => _.GeometricDataId)
            .Ignore();
        descriptor
            .Field(_ => _.GeometricData)
            .Ignore();
        descriptor
            .Field(_ => _.HygrothermalDataId)
            .Ignore();
        descriptor
            .Field(_ => _.HygrothermalData)
            .Ignore();
        descriptor
            .Field(_ => _.LifeCycleDataId)
            .Ignore();
        descriptor
            .Field(_ => _.LifeCycleData)
            .Ignore();
        descriptor
            .Field(_ => _.OpticalDataId)
            .Ignore();
        descriptor
            .Field(_ => _.OpticalData)
            .Ignore();
        descriptor
            .Field(_ => _.PhotovoltaicDataId)
            .Ignore();
        descriptor
            .Field(_ => _.PhotovoltaicData)
            .Ignore();
        descriptor
            .Field(_ => _.DataId)
            .Ignore();
        descriptor
            .Field(_ => _.DataKind)
            .Ignore();
        descriptor
            .Field(_ => _.Data)
            .Type<InterfaceType<IData>>()
            .Cost(0)
            .ResolveWith<Resolvers>(t =>
                Resolvers.GetData(default!, default!, default!));
        descriptor
            .Field(_ => _.UserAccessPolicies)
            .Cost(1)
            .ResolveWith<Resolvers>(_ => Resolvers.GetUserAccessPoliciesAsync(default!, default!, default!, default!, default!));
        descriptor
            .Field(_ => _.InstitutionAccessPolicies)
            .Cost(1)
            .ResolveWith<Resolvers>(_ => Resolvers.GetInstitutionAccessPoliciesAsync(default!, default!, default!, default!, default!));
        descriptor
            .Field(_ => _.OpenIdConnectApplicationAccessPolicies)
            .Cost(1)
            .ResolveWith<Resolvers>(_ => Resolvers.GetOpenIdConnectApplicationAccessPoliciesAsync(default!, default!, default!, default!, default!));
        descriptor
            .Field(_ => _.IsNobodyAllowed)
            .Cost(0)
            .ResolveWith<Resolvers>(t =>
                Resolvers.IsNobodyAllowedAsync(default!, default!, default!, default!));
        descriptor
            .Field(_ => _.IsAnyoneAllowed)
            .Cost(0)
            .ResolveWith<Resolvers>(t =>
                Resolvers.IsAnyoneAllowedAsync(default!, default!, default!, default!));
        descriptor
            .Field(_ => _.IsAccessAllowed(default!, default!, default!))
            .Cost(0)
            .ResolveWith<Resolvers>(t =>
                Resolvers.IsAccessAllowedAsync(default!, default!, default!, default!, default!, default!, default!));
    }

    private sealed class Resolvers
    {
        public static async Task<IData?> GetData(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IDataByIdAndKindDataLoader dataByIdAndKindDataLoader,
            CancellationToken cancellationToken
        )
        {
            if (dataAccessPolicy.DataId is null || dataAccessPolicy.DataKind is null)
            {
                return null;
            }
            return await dataByIdAndKindDataLoader.LoadRequiredAsync(
                (dataAccessPolicy.DataId ?? Guid.Empty, dataAccessPolicy.DataKind ?? default),
                cancellationToken
            );
        }

        public static async Task<bool> IsNobodyAllowedAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IDataAccessPolicyByDataIdDataLoader policyByDataIdDataLoader,
            ApplicationDbContext databaseContext,
            CancellationToken cancellationToken
        )
        {
            if (dataAccessPolicy.DataId is null)
            {
                return await databaseContext.DataAccessPolicies.AsQueryable()
                    .Where(_ => _.IsNobodyAllowed)
                    .SingleOrDefaultAsync(
                        _ => _.DataId == null,
                        cancellationToken
                    )
                    is not null;
            }
            return await policyByDataIdDataLoader
                .Where(_ => _.IsNobodyAllowed)
                .LoadAsync(
                    dataAccessPolicy.DataId ?? Guid.Empty,
                    cancellationToken
                )
                is not null;
        }

        public static Task<bool> IsAnyoneAllowedAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IDataAccessPolicyByDataIdDataLoader policyByDataIdDataLoader,
            ApplicationDbContext databaseContext,
            CancellationToken cancellationToken
        )
        {
            return IsAccessAllowedAsync(dataAccessPolicy, null, null, null, policyByDataIdDataLoader, databaseContext, cancellationToken);
        }

        public static async Task<bool> IsAccessAllowedAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            Guid? userId,
            Guid[]? institutionIds,
            string? openIdConnectClientId,
            IDataAccessPolicyByDataIdDataLoader policyByDataIdDataLoader,
            ApplicationDbContext databaseContext,
            CancellationToken cancellationToken
        )
        {
            if (dataAccessPolicy.DataId is null)
            {
                return await databaseContext.DataAccessPolicies.AsQueryable()
                    .Where(_ => _.IsAccessAllowed(userId, institutionIds, openIdConnectClientId))
                    .SingleOrDefaultAsync(
                        _ => _.DataId == null,
                        cancellationToken
                    )
                    is not null;
            }
            return await policyByDataIdDataLoader
                .Where(_ => _.IsAccessAllowed(userId, institutionIds, openIdConnectClientId))
                .LoadAsync(
                    dataAccessPolicy.DataId ?? Guid.Empty,
                    cancellationToken
                )
                is not null;
        }

        [UsePaging]
        [UseFiltering<UserAccessPolicyFilterType>]
        [UseSorting<UserAccessPolicySortType>]
        public static async Task<HotChocolate.Types.Pagination.Connection<UserAccessPolicy>> GetUserAccessPoliciesAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IResolverContext resolverContext,
            ApplicationDbContext databaseContext,
            CommonAuthorization authorization,
            CancellationToken cancellationToken
        )
        {
            if (!await authorization.IsDatabaseOperator(cancellationToken))
            {
                authorization.ReportUnauthorizedError(resolverContext);
                return HotChocolate.Types.Pagination.Connection.Empty<UserAccessPolicy>();
            }
            return await databaseContext.UserAccessPolicies
                .AsNoTracking()
                .Where(_ => _.DataAccessPolicyId == dataAccessPolicy.Id)
                .With(resolverContext.GetQueryContext<UserAccessPolicy>(), Sorting.DefaultEntityOrder)
                .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                .ToConnectionAsync();
        }

        [UsePaging]
        [UseFiltering<InstitutionAccessPolicyFilterType>]
        [UseSorting<InstitutionAccessPolicySortType>]
        public static async Task<HotChocolate.Types.Pagination.Connection<InstitutionAccessPolicy>> GetInstitutionAccessPoliciesAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IResolverContext resolverContext,
            ApplicationDbContext databaseContext,
            CommonAuthorization authorization,
            CancellationToken cancellationToken
        )
        {
            if (!await authorization.IsDatabaseOperator(cancellationToken))
            {
                authorization.ReportUnauthorizedError(resolverContext);
                return HotChocolate.Types.Pagination.Connection.Empty<InstitutionAccessPolicy>();
            }
            return await databaseContext.InstitutionAccessPolicies
                .AsNoTracking()
                .Where(_ => _.DataAccessPolicyId == dataAccessPolicy.Id)
                .With(resolverContext.GetQueryContext<InstitutionAccessPolicy>(), Sorting.DefaultEntityOrder)
                .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                .ToConnectionAsync();
        }

        [UsePaging]
        [UseFiltering<OpenIdConnectApplicationAccessPolicyFilterType>]
        [UseSorting<OpenIdConnectApplicationAccessPolicySortType>]
        public static async Task<HotChocolate.Types.Pagination.Connection<OpenIdConnectApplicationAccessPolicy>> GetOpenIdConnectApplicationAccessPoliciesAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IResolverContext resolverContext,
            ApplicationDbContext databaseContext,
            CommonAuthorization authorization,
            CancellationToken cancellationToken
        )
        {
            if (!await authorization.IsDatabaseOperator(cancellationToken))
            {
                authorization.ReportUnauthorizedError(resolverContext);
                return HotChocolate.Types.Pagination.Connection.Empty<OpenIdConnectApplicationAccessPolicy>();
            }
            return await databaseContext.OpenIdConnectApplicationAccessPolicies
                .AsNoTracking()
                .Where(_ => _.DataAccessPolicyId == dataAccessPolicy.Id)
                .With(resolverContext.GetQueryContext<OpenIdConnectApplicationAccessPolicy>(), Sorting.DefaultEntityOrder)
                .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                .ToConnectionAsync();
        }
    }
}