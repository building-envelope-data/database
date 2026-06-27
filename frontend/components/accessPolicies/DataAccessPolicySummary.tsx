import { Tag } from "antd";
import { humanize } from "../../lib/string";
import { DataAccessPolicyPartialFragment } from "../../queries/accessPolicies.generated";
import EntitySummary from "../entities/EntitySummary";
import { isTruthy } from "../../lib/array";

export default function DataAccessPolicySummary({
  entity,
}: {
  entity: DataAccessPolicyPartialFragment;
}) {
  return (
    <EntitySummary
      entity={{ name: "Data Access Policy", ...entity }}
      tags={[
        <Tag
          color="pink"
          style={{
            fontWeight: "normal",
          }}
        >
          {entity.data == null ? "global" : "local"}
          {/* <EntityLink entity={entity.data} route={() => paths.data(entity.data)} */}
        </Tag>,
        entity.isAnyoneAllowed && (
          <Tag key="status" style={{ fontWeight: "normal" }}>
            Anyone Allowed
          </Tag>
        ),
        entity.isNobodyAllowed && (
          <Tag key="status" style={{ fontWeight: "normal" }}>
            Nobody Allowed
          </Tag>
        ),
      ].filter(isTruthy)}
    >
      <div>Combinator: {humanize(entity.combinator, "all-upper")}</div>
    </EntitySummary>
  );
}
