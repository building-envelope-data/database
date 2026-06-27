import { Scalars } from "../../../__generated__/graphql";
import { HygrothermalDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Card, Button } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../../lib/hooks/useQueryHandler";
import HygrothermalDataSummary from "./HygrothermalDataSummary";
import QueryToolbar from "../../QueryToolbar";

interface HygrothermalDataProps {
  id: Scalars["Uuid"]["input"];
}

export default function HygrothermalData({ id }: HygrothermalDataProps) {
  const queryVariables = {
    id,
  };
  const { loading, error, data, refetch } = useQuery(HygrothermalDataDocument, {
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
        <HygrothermalDataSummary entity={entity} />
      </Card>
      <QueryToolbar
        query={HygrothermalDataDocument}
        variables={queryVariables}
      />
    </div>
  );
}
