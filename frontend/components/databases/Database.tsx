import { useDatabaseQuery } from "../../queries/databases.graphql";
import { Skeleton, Result, Descriptions, Typography, Tag } from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import { ReactNode, useEffect } from "react";
import { messageApolloError } from "../../lib/apollo";
import UpdateDatabase from "./UpdateDatabase";
import { DatabaseVerificationState } from "../../__generated__/__types__";

export type DatabaseProps = {};

export default function Database({}: DatabaseProps) {
  const { loading, error, data } = useDatabaseQuery();
  const database = data?.database;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!database) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <>
      <PageHeader
        title={database.name}
        subTitle={database.description}
        extra={([] as ReactNode[]).concat(
          database.canCurrentUserUpdateNode
            ? [
                <UpdateDatabase
                  key="updateDatabase"
                  databaseId={database.uuid}
                  name={database.name}
                  description={database.description}
                  locator={database.locator}
                />,
              ]
            : []
        )}
        tags={[
          <Tag key="verificationState" color="magenta">
            {database.verificationState}
          </Tag>,
        ]}
        backIcon={false}
      >
        <Descriptions size="small" column={1}>
          <Descriptions.Item label="UUID">{database.uuid}</Descriptions.Item>
          <Descriptions.Item label="Located at">
            <Typography.Link href={database.locator}>
              {database.locator}
            </Typography.Link>
          </Descriptions.Item>
        </Descriptions>
      </PageHeader>
      {database.canCurrentUserVerifyNode &&
        database.verificationState == DatabaseVerificationState.Pending && (
          <Typography.Paragraph>
            Have your database's GraphQL endpoint return the verification code "
            {database.verificationCode}" (without the quotation marks), when
            queried for the GraphQL query "verificationCode". Then, press the
            "Verify" button above to make the metabase assert that the
            verification codes match which proves that you control the GraphQL
            endpoint {database.locator}. Verified databases are publicly listed
            and included in data searches.
          </Typography.Paragraph>
        )}
    </>
  );
}
