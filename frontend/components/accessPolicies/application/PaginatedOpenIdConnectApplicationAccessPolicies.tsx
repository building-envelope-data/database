import {
  OpenIdConnectApplicationAccessPoliciesDocument,
  OpenIdConnectApplicationAccessPoliciesQueryVariables,
  OpenIdConnectApplicationAccessPoliciesPartialFragment,
} from "../../../queries/accessPolicies.generated";
import OpenIdConnectApplicationAccessPolicyList from "./OpenIdConnectApplicationAccessPolicyList";
import PaginatedEntities from "../../entities/PaginatedEntities";
import {
  OpenIdConnectApplicationAccessPolicyPropositionInput,
  OpenIdConnectApplicationAccessPolicySortInput,
} from "../../../__generated__/graphql";

export default function PaginatedOpenIdConnectApplicationAccessPolicies({
  where,
  order,
  extra,
}: {
  where?: OpenIdConnectApplicationAccessPoliciesQueryVariables["where"];
  order?: OpenIdConnectApplicationAccessPoliciesQueryVariables["order"];
  extra?: React.ReactNode;
}) {
  return (
    <PaginatedEntities<
      OpenIdConnectApplicationAccessPoliciesPartialFragment,
      OpenIdConnectApplicationAccessPolicyPropositionInput,
      OpenIdConnectApplicationAccessPolicySortInput
    >
      entitiesQuery={OpenIdConnectApplicationAccessPoliciesDocument}
      baseWhere={where}
      defaultOrder={order}
      showJump={false}
      extra={extra}
      list={(props) => <OpenIdConnectApplicationAccessPolicyList {...props} />}
      filterDefinitions={[
        {
          field: "clientId",
          type: "uuid",
        },
        {
          field: "dataAccessPolicy",
          type: "object",
          items: [
            {
              field: "dataId",
              type: "uuid",
            },
            // {
            //   field: "dataKind",
            //   type: "enum",
            //   enumObject: DataKind
            // }
          ],
        },
      ]}
      sortDefinitions={[
        { field: "clientId" },
        { field: "createdAt" },
        { field: "updatedAt" },
        { field: "id" },
      ]}
    />
  );
}
