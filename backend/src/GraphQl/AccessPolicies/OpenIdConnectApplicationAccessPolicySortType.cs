using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.AccessPolicies;

public sealed class OpenIdConnectApplicationAccessPolicySortType
    : AuditableEntitySortType<OpenIdConnectApplicationAccessPolicy>
{
    protected override void Configure(
        ISortInputTypeDescriptor<OpenIdConnectApplicationAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(OpenIdConnectApplicationAccessPolicySortType)[..^"SortType".Length] + GraphQlConstants.SortInputSuffix);
        descriptor.Field(_ => _.ClientId);
    }
}