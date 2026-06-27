import { Scalars } from "../../__generated__/graphql";
import paths from "../../paths";
import { OpenIdConnectApplicationAccessPolicyPartialFragment } from "../../queries/accessPolicies.generated";
import AccessPolicySummaryBase from "./AccessPolicySummaryBase";

const nameFallback = (id: Scalars["Uuid"]["output"]) => ({
  uuid: id,
  name: id,
});

export default function OpenIdConnectApplicationAccessPolicySummary({
  entity,
}: {
  entity: OpenIdConnectApplicationAccessPolicyPartialFragment;
}) {
  return (
    <AccessPolicySummaryBase
      entity={entity}
      stakeholder={
        entity.client?.name != null
          ? { uuid: entity.client.uuid, name: entity.client.name }
          : nameFallback(entity.clientId)
      }
      route={paths.metabase.openIdConnectApplication}
    />
  );
}
