import { Scalars } from "../../../__generated__/graphql";
import { CalorimetricDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result, Card, Button } from "antd";
import { useQuery } from "@apollo/client/react";
import { useQueryHandler } from "../../../lib/hooks/useQueryHandler";
import CalorimetricDataSummary from "./CalorimetricDataSummary";
import QueryToolbar from "../../QueryToolbar";

interface CalorimetricDataProps {
  id: Scalars["Uuid"]["input"];
}

export default function CalorimetricData({ id }: CalorimetricDataProps) {
  const queryVariables = {
    id,
  };
  const { loading, error, data, refetch } = useQuery(CalorimetricDataDocument, {
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
        <CalorimetricDataSummary entity={entity} />
      </Card>
      <QueryToolbar
        query={CalorimetricDataDocument}
        variables={queryVariables}
      />
    </div>
  );
}
