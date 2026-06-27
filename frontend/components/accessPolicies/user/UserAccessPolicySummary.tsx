import { Scalars } from "../../../__generated__/graphql";
import paths from "../../../paths";
import { UserAccessPoliciesPartialFragment } from "../../../queries/accessPolicies.generated";
import AccessPolicySummaryBase from "../AccessPolicySummaryBase";

const nameFallback = (id: Scalars["Uuid"]["output"]) => ({
  uuid: id,
  name: id,
});

export default function UserAccessPolicySummary({
  entity,
}: {
  entity: UserAccessPoliciesPartialFragment;
}) {
  return (
    <AccessPolicySummaryBase
      entity={entity}
      stakeholder={entity.user ?? nameFallback(entity.userId)}
      route={paths.metabase.user}
    />
  );
}
