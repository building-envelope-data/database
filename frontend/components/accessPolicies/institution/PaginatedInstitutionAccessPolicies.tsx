import {
  InstitutionAccessPoliciesDocument,
  InstitutionAccessPoliciesQueryVariables,
  InstitutionAccessPoliciesPartialFragment,
} from "../../../queries/accessPolicies.generated";
import InstitutionAccessPolicyList from "./InstitutionAccessPolicyList";
import PaginatedEntities from "../../entities/PaginatedEntities";
import {
  InstitutionAccessPolicyPropositionInput,
  InstitutionAccessPolicySortInput,
} from "../../../__generated__/graphql";

export default function PaginatedInstitutionAccessPolicies({
  where,
  order,
  extra,
}: {
  where?: InstitutionAccessPoliciesQueryVariables["where"];
  order?: InstitutionAccessPoliciesQueryVariables["order"];
  extra?: React.ReactNode;
}) {
  return (
    <PaginatedEntities<
      InstitutionAccessPoliciesPartialFragment,
      InstitutionAccessPolicyPropositionInput,
      InstitutionAccessPolicySortInput
    >
      entitiesQuery={InstitutionAccessPoliciesDocument}
      baseWhere={where}
      defaultOrder={order}
      showJump={false}
      extra={extra}
      list={(props) => <InstitutionAccessPolicyList {...props} />}
      filterDefinitions={[
        {
          field: "institutionId",
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
        { field: "institutionId" },
        { field: "createdAt" },
        { field: "updatedAt" },
        { field: "id" },
      ]}
    />
  );
}
