import {
  UserAccessPoliciesDocument,
  UserAccessPoliciesQueryVariables,
  UserAccessPoliciesPartialFragment,
} from "../../../queries/accessPolicies.generated";
import UserAccessPolicyList from "./UserAccessPolicyList";
import PaginatedEntities from "../../entities/PaginatedEntities";
import {
  UserAccessPolicyPropositionInput,
  UserAccessPolicySortInput,
} from "../../../__generated__/graphql";

export default function PaginatedUserAccessPolicies({
  where,
  order,
  extra,
}: {
  where?: UserAccessPoliciesQueryVariables["where"];
  order?: UserAccessPoliciesQueryVariables["order"];
  extra?: React.ReactNode;
}) {
  return (
    <PaginatedEntities<
      UserAccessPoliciesPartialFragment,
      UserAccessPolicyPropositionInput,
      UserAccessPolicySortInput
    >
      entitiesQuery={UserAccessPoliciesDocument}
      baseWhere={where}
      defaultOrder={order}
      showJump={false}
      extra={extra}
      list={(props) => <UserAccessPolicyList {...props} />}
      filterDefinitions={[
        {
          field: "userId",
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
        { field: "userId" },
        { field: "createdAt" },
        { field: "updatedAt" },
        { field: "id" },
      ]}
    />
  );
}
