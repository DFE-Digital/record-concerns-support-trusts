import { toDisplayDate } from "../../support/formatDate";
import CaseManagementPage from "../../pages/caseMangementPage";
import { Logger } from "cypress/common/logger";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import trustOverviewPage from "cypress/pages/trustOverviewPage";

describe("Trust overview ", () =>
{
    let caseId: string;
    let now: Date;

    describe("When we view a trust on a case", () =>
    {
        beforeEach(() => {
            cy.login();
            now = new Date();
    
            cy.basicCreateCase()
            .then((id: number) => {
                caseId = id + "";
            });
        });

        it("Should display trust details on case management and be able to create a case", () =>
        {
            //Only checking for the presence of the data, not the actual data becuase trust data may be sensitive/dynamic
            Logger.Log("Checking trust details are present");
            CaseManagementPage
                .viewTrustOverview();
    
            trustOverviewPage
                .trustTypeIsNotEmpty()
                .trustAddressIsNotEmpty()
                .trustAcademiesIsNotEmpty()
                .trustPupilCapacityIsNotEmpty()
                .trustPupilNumbersIsNotEmpty()
                .trustGroupIdIsNotEmpty()
                .trustUKPRNIsNotEmpty()
                .trustCompanyHouseNumberIsNotEmpty();
    
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
                });
        });
    });
});