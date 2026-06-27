import { Scalars } from "../../__generated__/graphql";
import {
  DataAccessPolicyDocument,
  DataAccessPolicyPartialFragment,
} from "../../queries/accessPolicies.generated";
import { Skeleton, Result, Card, Button } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../lib/hooks/useQueryHandler";
import DataAccessPolicySummary from "./DataAccessPolicySummary";
import EntityList from "../entities/EntityList";
import EntityItem from "../entities/EntityItem";
import LazyTabs, { LazyTabsProps } from "../LazyTabs";
import UserAccessPolicySummary from "./UserAccessPolicySummary";
import OpenIdConnectApplicationAccessPolicySummary from "./OpenIdConnectApplicationAccessPolicySummary";
import InstitutionAccessPolicySummary from "./InstitutionAccessPolicySummary";
import { useMemo } from "react";

const getTabs = (
  entity: DataAccessPolicyPartialFragment,
): LazyTabsProps["items"] => [
  {
    key: "userAccessPolicies",
    count: entity.userAccessPolicies.length,
    label: "User Access Policies",
    children: (
      <EntityList
        loading={false}
        dataSource={entity.userAccessPolicies}
        onReload={() => {}}
        renderItem={(node) => (
          <EntityItem>
            <UserAccessPolicySummary entity={node} />
          </EntityItem>
        )}
      />
    ),
  },
  {
    key: "institutionAccessPolicies",
    count: entity.institutionAccessPolicies.length,
    label: "Institution Access Policies",
    children: (
      <EntityList
        loading={false}
        dataSource={entity.institutionAccessPolicies}
        onReload={() => {}}
        renderItem={(node) => (
          <EntityItem>
            <InstitutionAccessPolicySummary entity={node} />
          </EntityItem>
        )}
      />
    ),
  },
  {
    key: "openIdConnectApplicationAccessPolicies",
    count: entity.openIdConnectApplicationAccessPolicies.length,
    label: "OpenID Connect Application Access Policies",
    children: (
      <EntityList
        loading={false}
        dataSource={entity.openIdConnectApplicationAccessPolicies}
        onReload={() => {}}
        renderItem={(node) => (
          <EntityItem>
            <OpenIdConnectApplicationAccessPolicySummary entity={node} />
          </EntityItem>
        )}
      />
    ),
  },
];

interface DataAccessPolicyProps {
  dataId: Scalars["Uuid"]["input"] | null | undefined;
}

export default function DataAccessPolicy({ dataId }: DataAccessPolicyProps) {
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
        <DataAccessPolicySummary entity={entity} />
      </Card>
      {/* <QueryToolbar */}
      {/*   query={DataAccessPolicyDocument} */}
      {/*   variables={queryVariables} */}
      {/* /> */}
      {tabs && <LazyTabs items={tabs} />}
    </div>
  );
}
