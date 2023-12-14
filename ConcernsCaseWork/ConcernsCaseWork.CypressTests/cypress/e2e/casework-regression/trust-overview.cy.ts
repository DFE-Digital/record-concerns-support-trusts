import { toDisplayDate } from "../../support/formatDate";
import { Logger } from "cypress/common/logger";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import trustOverviewPage from "cypress/pages/trustOverviewPage";
import { CreateCaseResponse } from "cypress/api/apiDomain";
import caseMangementPage from "cypress/pages/caseMangementPage";
import addToCasePage from "cypress/pages/caseActions/addToCasePage";
import { EditTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/editTrustFinancialForecastPage";
import { ViewTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/viewTrustFinancialForecastPage";
import { CloseTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/closeTrustFinancialForecastPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import closeConcernPage from "cypress/pages/closeConcernPage";


describe("Trust overview ", () =>
{
    let now: Date;
    let caseId: string;
    let trustUkprn: string;
    const editTFFPage = new EditTrustFinancialForecastPage();
	const viewTFFPage = new ViewTrustFinancialForecastPage();
	const closeTFFPage = new CloseTrustFinancialForecastPage();

    describe("When we view a trust on a case", () =>
    {
        beforeEach(() =>
        {
            cy.login();
            now = new Date();
        });

        describe("Concerns case", () =>
        {
            beforeEach(() => 
            {
                cy.basicCreateCase()
                .then((response: CreateCaseResponse) => {
                    caseId = response.urn + "";
                    trustUkprn = response.trustUkprn;
                });
            });
    
            it("Should display trust details on case management and the cases that belong to it", () =>
            {
                //Only checking for the presence of the data, not the actual data becuase trust data may be sensitive/dynamic
                Logger.log("Checking trust details are present");
                caseMangementPage
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
        
                Logger.log("Checking accessibility on Trust Overview");
                cy.excuteAccessibilityTests();	
        
                Logger.log("Checking case details are present on the trust overview page");
                caseworkTable
                    .getRowByCaseId(caseId)
                    .then((row) =>
                    {
                        row
                            .hasCaseId(caseId)
                            .hasCreatedDate(toDisplayDate(now))
                            .hasConcern("Financial compliance")
                            .hasRiskToTrust("Amber")
                            .hasRiskToTrust("Green")
                            .hasManagedBy("SFSO", "Midlands and West - West Midlands");
                    });
    
                Logger.log("Check on a closed case")
                caseMangementPage.viewCase();

                caseMangementPage.closeConcern();
                closeConcernPage.confirmCloseConcern();
    
                cy.closeCase();
                cy.visit(`/trust/${trustUkprn}/overview`);
    
                trustOverviewPage.showClosedCases();
    
                Logger.log("Checking closed case details are present on the trust overview page");
                caseworkTable
                .getRowByCaseId(caseId)
                .then((row) =>
                {
                    row
                        .hasCaseId(caseId)
                        .hasCreatedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                        .hasConcern("Financial compliance")
                        .hasManagedBy("SFSO", "Midlands and West - West Midlands");
                });
            });
        });

        describe("Non concerns case", () =>
        {
            beforeEach(() => 
            {
                cy.createNonConcernsCase()
                .then((response: CreateCaseResponse) => {
                    caseId = response.urn + "";
                    trustUkprn = response.trustUkprn;
                });
            });

            it("Should display the content correctly for a non concerns case with case actions", () =>
            {
                caseMangementPage.getAddToCaseBtn().click();
                addToCasePage.addToCase("TrustFinancialForecast");
                addToCasePage.getAddToCaseBtn().click();
    
                editTFFPage.save();
    
                caseMangementPage
                    .viewTrustOverview();
    
                caseworkTable
                    .getRowByCaseId(caseId)
                    .then((row) =>
                    {
                        row
                            .hasAction("Action: TFF (trust financial forecast)");
                    });
    
                Logger.log("Check on a closed non concerns case")
                caseMangementPage.viewCase();
    
                actionSummaryTable
                    .getOpenAction("TFF (trust financial forecast)")
                    .then((row) => {
                        row.select();
                    });
    
                viewTFFPage.close();
                closeTFFPage.close();
    
                cy.closeCase();
                cy.visit(`/trust/${trustUkprn}/overview`);
    
                trustOverviewPage.showClosedCases();
    
                caseworkTable
                .getRowByCaseId(caseId)
                .then((row) =>
                {
                    row
                        .hasAction("Action: TFF (trust financial forecast)");
                });
            });
        });
    });
});