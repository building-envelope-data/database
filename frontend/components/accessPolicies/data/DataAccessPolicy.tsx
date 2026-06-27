import { Scalars, SortEnumType } from "../../../__generated__/graphql";
import {
  DataAccessPolicyDocument,
  DataAccessPolicyPartialFragment,
} from "../../../queries/accessPolicies.generated";
import { Skeleton, Result, Card, Button } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../../lib/hooks/useQueryHandler";
import DataAccessPolicySummary from "./DataAccessPolicySummary";
import LazyTabs, { LazyTabsProps } from "../../LazyTabs";
import { useMemo } from "react";
import PaginatedUserAccessPolicies from "../user/PaginatedUserAccessPolicies";
import PaginatedOpenIdConnectApplicationAccessPolicies from "../application/PaginatedOpenIdConnectApplicationAccessPolicies";
import PaginatedInstitutionAccessPolicies from "../institution/PaginatedInstitutionAccessPolicies";

const getTabs = (
  entity: DataAccessPolicyPartialFragment,
): LazyTabsProps["items"] => [
  {
    key: "userAccessPolicies",
    count: entity.userAccessPolicies?.totalCount,
    label: "User Access Policies",
    children: (
      <PaginatedUserAccessPolicies
        where={{
          dataAccessPolicy: {
            id: { equalTo: entity.uuid },
          },
        }}
        order={{ createdAt: SortEnumType.Desc }}
      />
    ),
  },
  {
    key: "institutionAccessPolicies",
    count: entity.institutionAccessPolicies?.totalCount,
    label: "Institution Access Policies",
    children: (
      <PaginatedInstitutionAccessPolicies
        where={{
          dataAccessPolicy: {
            id: { equalTo: entity.uuid },
          },
        }}
        order={{ createdAt: SortEnumType.Desc }}
      />
    ),
  },
  {
    key: "openIdConnectApplicationAccessPolicies",
    count: entity.openIdConnectApplicationAccessPolicies?.totalCount,
    label: "OpenID Connect Application Access Policies",
    children: (
      <PaginatedOpenIdConnectApplicationAccessPolicies
        where={{
          dataAccessPolicy: {
            id: { equalTo: entity.uuid },
          },
        }}
        order={{ createdAt: SortEnumType.Desc }}
      />
    ),
  },
];

interface DataAccessPolicyProps {
  dataId: Scalars["Uuid"]["input"] | null | undefined;
  onDataPage?: boolean;
}

export default function DataAccessPolicy({
  dataId,
  onDataPage,
}: DataAccessPolicyProps) {
  const queryVariables = {
    dataId,
  };
  const { loading, error, data, refetch } = useQuery(DataAccessPolicyDocument, {
    variables: queryVariables,
  });
  useQueryHandler({ error });

  const entity = data?.data;
  const tabs = useMemo(() => (!entity ? null : getTabs(entity)), [entity]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!entity) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
        extra={
          <Button loading={loading} onClick={() => refetch()}>
            Reload
          </Button>
        }
      />
    );
  }

  return (
    <div>
      <Card style={{ marginBottom: "1em" }}>
        <DataAccessPolicySummary entity={entity} onDataPage={onDataPage} />
      </Card>
      {/* <QueryToolbar */}
      {/*   query={DataAccessPolicyDocument} */}
      {/*   variables={queryVariables} */}
      {/* /> */}
      {tabs && <LazyTabs items={tabs} />}
    </div>
  );
}
