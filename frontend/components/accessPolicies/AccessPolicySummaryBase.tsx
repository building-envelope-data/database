import { Statistic, Tag } from "antd";
import {
  InstitutionAccessPoliciesPartialFragment,
  OpenIdConnectApplicationAccessPoliciesPartialFragment,
  UserAccessPoliciesPartialFragment,
} from "../../queries/accessPolicies.generated";
import EntitySummary from "../entities/EntitySummary";
import dayjs from "dayjs";
import durationPlugin from "dayjs/plugin/duration";
import relativeTime from "dayjs/plugin/relativeTime";
import { Route } from "next";
import { Scalars } from "../../__generated__/graphql";
import { isTruthy } from "../../lib/array";
import DateTimeX from "../DateTimeX";

dayjs.extend(durationPlugin);
dayjs.extend(relativeTime);

export default function AccessPolicySummaryBase({
  entity,
  stakeholder,
  route,
}: {
  entity:
    | UserAccessPoliciesPartialFragment
    | InstitutionAccessPoliciesPartialFragment
    | OpenIdConnectApplicationAccessPoliciesPartialFragment;
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
      entity={stakeholder}
      route={route}
      tags={[
        entity.isAlwaysAllowed && (
          <Tag key="status" style={{ fontWeight: "normal" }}>
            always allowed
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
        <div>
          {duration != null && startTime == null ? (
            <>within {duration.humanize()}</>
          ) : (
            <>
              from{" "}
              {startTime == null ? "-∞" : <DateTimeX parsedValue={startTime} />}{" "}
              until{" "}
              {endTime == null ? "-∞" : <DateTimeX parsedValue={endTime} />}
            </>
          )}
        </div>
      </div>
    </EntitySummary>
  );
}
