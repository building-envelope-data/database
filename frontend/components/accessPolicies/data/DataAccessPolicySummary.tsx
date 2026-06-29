import { Tag } from "antd";
import { humanize } from "../../../lib/string";
import { DataAccessPolicyPartialFragment } from "../../../queries/accessPolicies.generated";
import EntitySummary from "../../entities/EntitySummary";
import { isTruthy } from "../../../lib/array";
import paths from "../../../paths";
import EnumTag from "../../EnumTag";
import Copyable from "../../Copyable";

export default function DataAccessPolicySummary({
  entity,
  onDataPage = false,
}: {
  entity: DataAccessPolicyPartialFragment;
  onDataPage?: boolean;
}) {
  return (
    <EntitySummary
      entity={{
        uuid: entity.data?.uuid,
        name:
          onDataPage || entity.data == null
            ? "Data Access Policy"
            : entity.data.name,
        description:
          entity.data == null
            ? "applies to all data sets"
            : onDataPage
              ? "applies to the present data set"
              : `data access policy for the named data set with ${entity.userAccessPolicies?.totalCount ?? 0} user, ${entity.institutionAccessPolicies?.totalCount ?? 0} institution, and ${entity.openIdConnectApplicationAccessPolicies?.totalCount ?? 0} OpenID Connect application access policies. Follow the link for details…`,
      }}
      route={
        entity.data == null
          ? undefined
          : (id) => paths.data(entity.data?.kind!, id)
      }
      tags={[
        (onDataPage || (!onDataPage && entity.isGlobal)) && (
          <Tag
            key="isGlobal"
            color={entity.isGlobal ? "pink" : "orange"}
            style={{
              fontWeight: "normal",
            }}
          >
            {entity.isGlobal ? "global" : "local"}
          </Tag>
        ),
        entity.isEveryoneAllowed && (
          <Tag key="isEveryoneAllowed" style={{ fontWeight: "normal" }}>
            everyone allowed
          </Tag>
        ),
        entity.isNobodyAllowed && (
          <Tag key="isNobodyAllowed" style={{ fontWeight: "normal" }}>
            nobody allowed
          </Tag>
        ),
        entity.data && (
          <Copyable onlyIcon text={entity.data.kind}>
            <EnumTag key="kind">{entity.data.kind}</EnumTag>
          </Copyable>
        ),
      ].filter(isTruthy)}
    >
      <div>Combinator: {humanize(entity.combinator, "all-upper")}</div>
    </EntitySummary>
  );
}
