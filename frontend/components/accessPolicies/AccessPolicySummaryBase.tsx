import { Statistic, Tag } from "antd";
import {
  InstitutionAccessPolicyPartialFragment,
  OpenIdConnectApplicationAccessPolicyPartialFragment,
  UserAccessPolicyPartialFragment,
} from "../../queries/accessPolicies.generated";
import EntitySummary from "../entities/EntitySummary";
import dayjs from "dayjs";
import durationPlugin from "dayjs/plugin/duration";
import { Route } from "next";
import { Scalars } from "../../__generated__/graphql";
import { isTruthy } from "../../lib/array";

dayjs.extend(durationPlugin);

export default function AccessPolicySummaryBase({
  entity,
  stakeholder,
  route,
}: {
  entity:
    | UserAccessPolicyPartialFragment
    | InstitutionAccessPolicyPartialFragment
    | OpenIdConnectApplicationAccessPolicyPartialFragment;
  stakeholder: {
    uuid: Scalars["Uuid"]["output"];
    name?: string | null;
  };
  route?: (id: Scalars["Uuid"]["output"]) => Route;
}) {
  const duration =
    entity.upperAccessLimitPerTimeDuration?.duration == null
      ? null
      : dayjs.duration(entity.upperAccessLimitPerTimeDuration?.duration);
  const startTime =
    entity.accessCountSinceStartTime?.startTime == null
      ? null
      : dayjs(entity.accessCountSinceStartTime?.startTime);
  const endTime = duration == null ? null : startTime?.add(duration);

  return (
    <EntitySummary
      entity={{ ...entity, ...stakeholder }}
      route={route}
      tags={[
        entity.isAlwaysAllowed && (
          <Tag key="status" style={{ fontWeight: "normal" }}>
            Always Allowed
          </Tag>
        ),
      ].filter(isTruthy)}
    >
      <div>
        <Statistic
          title="Access Count"
          value={entity.accessCountSinceStartTime?.accessCount ?? 0}
          suffix={`of ${entity.upperAccessLimitPerTimeDuration?.upperLimit ?? "∞"}`}
        />
        from {startTime?.format("YYYY-MM-DD HH:mm") ?? "-∞"} until{" "}
        {endTime?.format("YYYY-MM-DD HH:mm") ?? "∞"}
      </div>
    </EntitySummary>
  );
}
