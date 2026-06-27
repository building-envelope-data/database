using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class UserAccessPolicySortType
    : AuditableEntitySortType<UserAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<UserAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(UserAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.UserId);
    }
}