import activeCaseworkTable from "../../pages/yourCasework/activeCaseworkTable";
import CaseManagementPage from "../../pages/createCase/managementPage";
import { toDisplayDate } from "../../support/formatDate";

describe("Your casework tests", () =>
{
    let caseId: string;
    let trustName: string;
    let caseManagementPage = new CaseManagementPage();
    let now: Date;

    beforeEach(() => {
		cy.login();
        now = new Date();

        cy.basicCreateCase()
        .then((id: number) => {
            caseId = id + "";
            return caseManagementPage.getTrust()
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