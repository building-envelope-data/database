import { Divider, Tag, Typography } from "antd";
import Layout from "../../components/Layout";
import DataAccessPolicy from "../../components/accessPolicies/data/DataAccessPolicy";
import PaginatedDataAccessPolicies from "../../components/accessPolicies/data/PaginatedDataAccessPolicies";
import { SortEnumType } from "../../__generated__/graphql";
import LazyTabs, { LazyTabsProps } from "../../components/LazyTabs";
import PaginatedOpenIdConnectApplicationAccessPolicies from "../../components/accessPolicies/application/PaginatedOpenIdConnectApplicationAccessPolicies";
import PaginatedInstitutionAccessPolicies from "../../components/accessPolicies/institution/PaginatedInstitutionAccessPolicies";
import PaginatedUserAccessPolicies from "../../components/accessPolicies/user/PaginatedUserAccessPolicies";

const tabs: LazyTabsProps["items"] = [
  {
    key: "userAccessPolicies",
    label: "User Access Policies",
    children: (
      <PaginatedUserAccessPolicies order={{ createdAt: SortEnumType.Desc }} />
    ),
  },
  {
    key: "institutionAccessPolicies",
    label: "Institution Access Policies",
    children: (
      <PaginatedInstitutionAccessPolicies
        order={{ createdAt: SortEnumType.Desc }}
      />
    ),
  },
  {
    key: "openIdConnectApplicationAccessPolicies",
    label: "OpenID Connect Application Access Policies",
    children: (
      <PaginatedOpenIdConnectApplicationAccessPolicies
        order={{ createdAt: SortEnumType.Desc }}
      />
    ),
  },
];

export default function Page() {
  return (
    <Layout>
      <DataAccessPolicy dataId={null} />
      <Divider />
      <Typography.Title level={4}>
        Data Access Policies{" "}
        <Tag color="orange" style={{ fontWeight: "normal" }}>
          local
        </Tag>
      </Typography.Title>
      <Typography.Text type="secondary">
        the once that do not allow access to everyone
      </Typography.Text>
      <PaginatedDataAccessPolicies
        where={{
          and: [
            { isGlobal: { equalTo: false } },
            { isAnyoneAllowed: { equalTo: false } },
          ],
        }}
      />
      <Divider />
      <Typography.Title level={4}>
        User, Institution, and OpenID Connect Application Policies
      </Typography.Title>
      <Typography.Text type="secondary">
        each belonging to some local data policy
      </Typography.Text>
      <LazyTabs items={tabs} />
    </Layout>
  );
}
