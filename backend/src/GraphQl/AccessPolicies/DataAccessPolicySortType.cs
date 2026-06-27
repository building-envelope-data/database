using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class DataAccessPolicySortType
    : AuditableEntitySortType<DataAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(DataAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.DataId);
        descriptor.Field(_ => _.DataKind);
    }
}