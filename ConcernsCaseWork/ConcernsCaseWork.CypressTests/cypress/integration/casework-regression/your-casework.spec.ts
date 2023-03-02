import activeCaseworkTable from "../../pages/yourCasework/activeCaseworkTable";
import { toDisplayDate } from "../../support/formatDate";
import CaseManagementPage from "../../pages/caseMangementPage";

describe("Your casework tests", () =>
{
    let caseId: string;
    let trustName: string;
    let now: Date;

    beforeEach(() => {
		//cy.login();
cy.visit("/");
        now = new Date();

        cy.basicCreateCase()
        .then((id: number) => {
            caseId = id + "";
            return CaseManagementPage.getTrust()
        })
        .then((trust: string) =>
        {
            trustName = trust.trim();
            cy.visit("/");
        });
	});

    describe("When we create a case", () =>
    {
        it("Should appear in the your casework section", () =>
        {
            activeCaseworkTable
                .getRowByCaseId(caseId)
                .then((row) =>
                {
                    row
                        .hasCaseId(caseId)
                        .hasCreatedDate(toDisplayDate(now))
                        .hasTrust(trustName)
                        .hasConcern("Governance and compliance: Compliance")
                        .hasRiskToTrust("Amber")
                        .hasRiskToTrust("Green")
                })
        });
    });
})