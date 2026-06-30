import {
  DataAccessPoliciesDocument,
  DataAccessPoliciesQueryVariables,
  DataAccessPoliciesPartialFragment,
} from "../../../queries/accessPolicies.generated";
import DataAccessPolicyList from "./DataAccessPolicyList";
import PaginatedEntities from "../../entities/PaginatedEntities";
import {
  DataAccessPolicyPropositionInput,
  DataAccessPolicySortInput,
} from "../../../__generated__/graphql";

export default function PaginatedDataAccessPolicies({
  where,
  order,
  extra,
}: {
  where?: DataAccessPoliciesQueryVariables["where"];
  order?: DataAccessPoliciesQueryVariables["order"];
  extra?: React.ReactNode;
}) {
  return (
    <PaginatedEntities<
      DataAccessPoliciesPartialFragment,
      DataAccessPolicyPropositionInput,
      DataAccessPolicySortInput
    >
      entitiesQuery={DataAccessPoliciesDocument}
      baseWhere={where}
      defaultOrder={order}
      showJump={false}
      extra={extra}
      list={(props) => <DataAccessPolicyList {...props} />}
      filterDefinitions={[
        {
          field: "dataId",
          type: "uuid",
        },
        // {
        //   field: "dataKind",
        //   type: "enum",
        //   enumObject: DataKind
        // },
      ]}
      sortDefinitions={[
        { field: "dataId" },
        { field: "dataKind" },
        { field: "createdAt" },
        { field: "updatedAt" },
        { field: "id" },
      ]}
    />
  );
}
