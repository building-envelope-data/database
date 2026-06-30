using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data.AccessPolicies;
using Database.Extensions;
using GreenDonut;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class UserAccessPolicyType
    : AccessPolicyTypeBase<UserAccessPolicy, IUserAccessPolicyByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<UserAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(_ => _.DataAccessPolicy)
            .Type<NonNullType<ObjectType<DataAccessPolicy>>>()
            .ResolveWith<Resolvers>(_ => Resolvers.GetDataAccessPolicyAsync(default!, default!));
        descriptor
            .Field(nameof(UserAccessPolicy.UserId)[..^2].FirstCharToLower())
            .Type<ObjectType<UserDataLoader.User>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetUserAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<DataAccessPolicy> GetDataAccessPolicyAsync(
            [Parent] UserAccessPolicy parent,
            IDataAccessPolicyByIdDataLoader byId
        )
        {
            return byId.LoadRequiredAsync(parent.DataAccessPolicyId);
        }

        public static Task<UserDataLoader.User?> GetUserAsync(
            [Parent] UserAccessPolicy parent,
            IUserByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.UserId);
        }
    }
}