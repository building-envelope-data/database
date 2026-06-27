using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data.AccessPolicies;
using Database.Extensions;
using GreenDonut;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class InstitutionAccessPolicyType
    : AccessPolicyTypeBase<InstitutionAccessPolicy, IInstitutionAccessPolicyByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<InstitutionAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(_ => _.DataAccessPolicy)
            .Type<NonNullType<ObjectType<DataAccessPolicy>>>()
            .ResolveWith<Resolvers>(_ => Resolvers.GetDataAccessPolicyAsync(default!, default!));
        descriptor
            .Field(nameof(InstitutionAccessPolicy.InstitutionId)[..^2].FirstCharToLower())
            .Type<ObjectType<InstitutionDataLoader.Institution>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetInstitutionAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<DataAccessPolicy> GetDataAccessPolicyAsync(
            [Parent] InstitutionAccessPolicy parent,
            IDataAccessPolicyByIdDataLoader byId
        )
        {
            return byId.LoadRequiredAsync(parent.DataAccessPolicyId);
        }

        public static Task<InstitutionDataLoader.Institution?> GetInstitutionAsync(
            [Parent] InstitutionAccessPolicy parent,
            IInstitutionByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.InstitutionId);
        }
    }
}