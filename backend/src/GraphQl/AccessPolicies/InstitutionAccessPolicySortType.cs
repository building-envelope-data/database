using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class InstitutionAccessPolicySortType
    : AuditableEntitySortType<InstitutionAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<InstitutionAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(InstitutionAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.InstitutionId);
    }
}