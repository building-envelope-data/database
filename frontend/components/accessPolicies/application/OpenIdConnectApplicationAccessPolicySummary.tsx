import { Scalars } from "../../../__generated__/graphql";
import paths from "../../../paths";
import { OpenIdConnectApplicationAccessPoliciesPartialFragment } from "../../../queries/accessPolicies.generated";
import AccessPolicySummaryBase from "../AccessPolicySummaryBase";

const nameFallback = (id: Scalars["Uuid"]["output"]) => ({
  uuid: id,
  name: id,
});

export default function OpenIdConnectApplicationAccessPolicySummary({
  entity,
}: {
  entity: OpenIdConnectApplicationAccessPoliciesPartialFragment;
}) {
  return (
    <AccessPolicySummaryBase
      entity={entity}
      stakeholder={entity.client ?? nameFallback(entity.clientId)}
      route={paths.metabase.openIdConnectApplication}
    />
  );
}
