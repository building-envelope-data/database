import { DataAccessPoliciesPartialFragment } from "../../../queries/accessPolicies.generated";
import EntityItem from "../../entities/EntityItem";
import EntityList from "../../entities/EntityList";
import DataAccessPolicySummary from "./DataAccessPolicySummary";

export default function DataAccessPolicyList({
  loading,
  nodes,
  onReload,
}: {
  loading: boolean;
  nodes: DataAccessPoliciesPartialFragment[] | null;
  onReload: () => void;
}) {
  return (
    <EntityList
      loading={loading}
      dataSource={nodes}
      onReload={onReload}
      renderItem={(node) => (
        <EntityItem>
          <DataAccessPolicySummary entity={node} />
        </EntityItem>
      )}
    />
  );
}
