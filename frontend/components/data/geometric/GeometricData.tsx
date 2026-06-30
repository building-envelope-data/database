import { Scalars } from "../../../__generated__/graphql";
import { GeometricDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Card, Button } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../../lib/hooks/useQueryHandler";
import GeometricDataSummary from "./GeometricDataSummary";
import QueryToolbar from "../../QueryToolbar";
import LocalAndGlobalDataAccessPolicies from "../../accessPolicies/LocalAndGlobalDataAccessPolicies";

interface GeometricDataProps {
  id: Scalars["Uuid"]["input"];
}

export default function GeometricData({ id }: GeometricDataProps) {
  const queryVariables = {
    id,
  };
  const { loading, error, data, refetch } = useQuery(GeometricDataDocument, {
    variables: queryVariables,
  });
  useQueryHandler({ error });
  const entity = data?.data;

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
        <GeometricDataSummary entity={entity} />
      </Card>
      <QueryToolbar query={GeometricDataDocument} variables={queryVariables} />
      <LocalAndGlobalDataAccessPolicies dataId={entity.uuid} />
    </div>
  );
}
