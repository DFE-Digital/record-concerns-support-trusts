import { toDisplayDate } from "../../support/formatDate";
import CaseManagementPage from "../../pages/caseMangementPage";
import { Logger } from "cypress/common/logger";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";

describe("Trust overview ", () =>
{
    let caseId: string;
    let now: Date;

    beforeEach(() => {
		cy.login();
        now = new Date();

        cy.basicCreateCase()
        .then((id: number) => {
            caseId = id + "";
            return CaseManagementPage.getTrust()
        })
	});

    describe("When we create a case", () =>
    {
        it("Should display trust details", () =>
        {
            //Only checking for the presence of the data, not the actual data becuase trust data may be sensitive/dynamic
            Logger.Log("Checking trust details are present");
            CaseManagementPage
                .viewTrustOverview()
                .trustTypeIsNotEmpty()
                .trustAddressIsNotEmpty()
                .trustAcademiesIsNotEmpty()
                .trustPupilCapacityIsNotEmpty()
                .trustPupilNumbersIsNotEmpty()
                .trustGroupIdIsNotEmpty()
                .trustUKPRNIsNotEmpty()
                .trustCompanyHouseNumberIsNotEmpty()

            Logger.Log("Checking accessibility on Trust Overview");
            cy.excuteAccessibilityTests();	

            Logger.Log("Checking case details are present on the trust overview page");
            caseworkTable
                .getRowByCaseId(caseId)
                .then((row) =>
                {
                    row
                        .hasCaseId(caseId)
                        .hasCreatedDate(toDisplayDate(now))
                        .hasConcern("Governance and compliance: Compliance")
                        .hasRiskToTrust("Amber")
                        .hasRiskToTrust("Green")
                })
        });
    });
})