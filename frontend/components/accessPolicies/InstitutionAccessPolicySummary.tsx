import { Scalars } from "../../__generated__/graphql";
import paths from "../../paths";
import { InstitutionAccessPolicyPartialFragment } from "../../queries/accessPolicies.generated";
import AccessPolicySummaryBase from "./AccessPolicySummaryBase";

const nameFallback = (id: Scalars["Uuid"]["output"]) => ({
  uuid: id,
  name: id,
});

export default function InstitutionAccessPolicySummary({
  entity,
}: {
  entity: InstitutionAccessPolicyPartialFragment;
}) {
  return (
    <AccessPolicySummaryBase
      entity={entity}
      stakeholder={entity.institution ?? nameFallback(entity.institutionId)}
      route={paths.metabase.institution}
    />
  );
}
