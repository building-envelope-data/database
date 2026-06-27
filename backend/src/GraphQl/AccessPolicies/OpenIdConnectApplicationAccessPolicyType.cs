using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data.AccessPolicies;
using Database.Extensions;
using GreenDonut;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class OpenIdConnectApplicationAccessPolicyType
    : AccessPolicyTypeBase<OpenIdConnectApplicationAccessPolicy, IOpenIdConnectApplicationAccessPolicyByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<OpenIdConnectApplicationAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(_ => _.DataAccessPolicyId)
            .Ignore();
        descriptor
            .Field(_ => _.DataAccessPolicy)
            .Type<NonNullType<ObjectType<DataAccessPolicy>>>()
            .ResolveWith<Resolvers>(_ => Resolvers.GetDataAccessPolicyAsync(default!, default!));
        descriptor
            .Field(nameof(OpenIdConnectApplicationAccessPolicy.ClientId)[..^2].FirstCharToLower())
            .Type<ObjectType<OpenIdConnectApplicationDataLoader.OpenIdConnectApplication>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetOpenIdConnectApplicationAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<DataAccessPolicy> GetDataAccessPolicyAsync(
            [Parent] OpenIdConnectApplicationAccessPolicy parent,
            IDataAccessPolicyByIdDataLoader byId
        )
        {
            return byId.LoadRequiredAsync(parent.DataAccessPolicyId);
        }

        public static Task<OpenIdConnectApplicationDataLoader.OpenIdConnectApplication?> GetOpenIdConnectApplicationAsync(
            [Parent] OpenIdConnectApplicationAccessPolicy parent,
            IOpenIdConnectApplicationByClientIdDataLoader byClientId
        )
        {
            return byClientId.LoadAsync(parent.ClientId);
        }
    }
}