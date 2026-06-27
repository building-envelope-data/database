import { OpenIdConnectApplicationAccessPoliciesPartialFragment } from "../../../queries/accessPolicies.generated";
import EntityItem from "../../entities/EntityItem";
import EntityList from "../../entities/EntityList";
import OpenIdConnectApplicationAccessPolicySummary from "./OpenIdConnectApplicationAccessPolicySummary";

export default function OpenIdConnectApplicationAccessPolicyList({
  loading,
  nodes,
  onReload,
}: {
  loading: boolean;
  nodes: OpenIdConnectApplicationAccessPoliciesPartialFragment[] | null;
  onReload: () => void;
}) {
  return (
    <EntityList
      loading={loading}
      dataSource={nodes}
      onReload={onReload}
      renderItem={(node) => (
        <EntityItem>
          <OpenIdConnectApplicationAccessPolicySummary entity={node} />
        </EntityItem>
      )}
    />
  );
}
