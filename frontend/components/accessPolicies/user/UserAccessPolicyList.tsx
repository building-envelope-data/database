import { UserAccessPoliciesPartialFragment } from "../../../queries/accessPolicies.generated";
import EntityItem from "../../entities/EntityItem";
import EntityList from "../../entities/EntityList";
import UserAccessPolicySummary from "./UserAccessPolicySummary";

export default function UserAccessPolicyList({
  loading,
  nodes,
  onReload,
}: {
  loading: boolean;
  nodes: UserAccessPoliciesPartialFragment[] | null;
  onReload: () => void;
}) {
  return (
    <EntityList
      loading={loading}
      dataSource={nodes}
      onReload={onReload}
      renderItem={(node) => (
        <EntityItem>
          <UserAccessPolicySummary entity={node} />
        </EntityItem>
      )}
    />
  );
}
