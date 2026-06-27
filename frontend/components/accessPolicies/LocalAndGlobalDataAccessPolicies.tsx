import { Scalars } from "../../__generated__/graphql";
import { Divider } from "antd";
import { useQuery } from "@apollo/client/react";
import DataAccessPolicy from "../accessPolicies/data/DataAccessPolicy";
import { CurrentUserDocument } from "../../queries/currentUser.generated";

export default function LocalAndGlobalDataAccessPolicies({
  dataId,
}: {
  dataId: Scalars["Uuid"]["input"];
}) {
  const currentUserData = useQuery(CurrentUserDocument)?.data;
  const currentUser = currentUserData?.currentUser;

  return (
    currentUser?.isAtLeastAssistantManagerOfDatabaseOperator && (
      <>
        <Divider />
        <DataAccessPolicy onDataPage dataId={dataId} />
        <Divider />
        <DataAccessPolicy onDataPage dataId={null} />
      </>
    )
  );
}
