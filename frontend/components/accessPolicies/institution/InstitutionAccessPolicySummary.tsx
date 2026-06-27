import { Scalars } from "../../../__generated__/graphql";
import paths from "../../../paths";
import { InstitutionAccessPoliciesPartialFragment } from "../../../queries/accessPolicies.generated";
import AccessPolicySummaryBase from "../AccessPolicySummaryBase";

const nameFallback = (id: Scalars["Uuid"]["output"]) => ({
  uuid: id,
  name: id,
});

export default function InstitutionAccessPolicySummary({
  entity,
}: {
  entity: InstitutionAccessPoliciesPartialFragment;
}) {
  return (
    <AccessPolicySummaryBase
      entity={entity}
      stakeholder={entity.institution ?? nameFallback(entity.institutionId)}
      route={paths.metabase.institution}
    />
  );
}
