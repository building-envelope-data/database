import { Scalars } from "../../../__generated__/graphql";
import { OpticalDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Card, Button } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../../lib/hooks/useQueryHandler";
import OpticalDataSummary from "./OpticalDataSummary";
import OpticalDataRibbon from "./OpticalDataRibbon";
import QueryToolbar from "../../QueryToolbar";
import LocalAndGlobalDataAccessPolicies from "../../accessPolicies/LocalAndGlobalDataAccessPolicies";

interface OpticalDataProps {
  id: Scalars["Uuid"]["input"];
}

export default function OpticalData({ id }: OpticalDataProps) {
  const queryVariables = {
    id,
  };
  const { loading, error, data, refetch } = useQuery(OpticalDataDocument, {
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
      <OpticalDataRibbon {...entity}>
        <Card style={{ marginBottom: "1em" }}>
          <OpticalDataSummary entity={entity} />
        </Card>
      </OpticalDataRibbon>
      <QueryToolbar query={OpticalDataDocument} variables={queryVariables} />
      <LocalAndGlobalDataAccessPolicies dataId={entity.uuid} />
    </div>
  );
}
