import { InstitutionAccessPoliciesPartialFragment } from "../../../queries/accessPolicies.generated";
import EntityItem from "../../entities/EntityItem";
import EntityList from "../../entities/EntityList";
import InstitutionAccessPolicySummary from "./InstitutionAccessPolicySummary";

export default function InstitutionAccessPolicyList({
  loading,
  nodes,
  onReload,
}: {
  loading: boolean;
  nodes: InstitutionAccessPoliciesPartialFragment[] | null;
  onReload: () => void;
}) {
  return (
    <EntityList
      loading={loading}
      dataSource={nodes}
      onReload={onReload}
      renderItem={(node) => (
        <EntityItem>
          <InstitutionAccessPolicySummary entity={node} />
        </EntityItem>
      )}
    />
  );
}
