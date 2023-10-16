import { Logger } from "cypress/common/logger";
import caseMangementPage from "cypress/pages/caseMangementPage";
import closedCasePage from "cypress/pages/closedCasePage";
import { ViewClosedCasePage } from "cypress/pages/createCase/viewClosedCasePage";
import editConcernPage from "cypress/pages/editConcernPage";
import homePage from "cypress/pages/homePage";

const viewClosedCasePage = new ViewClosedCasePage();

describe("Case without a division", () =>
{
    describe("Any cases created prior to Feb 23 will not have a division", () =>
    {
        let caseId: number;

        beforeEach(() =>
        {
            cy.login();

            cy.basicCreateCase().then((caseResponse) => {
                caseId = caseResponse.urn;
            });
        })

        it("Should show how a case looks open and closed without a division", () =>
        {
            Logger.Log("Managed by won't have the division set");
            caseMangementPage.hasManagedBy("Empty", "Midlands and West - West Midlands");

            caseMangementPage.editConcern();
            editConcernPage.closeConcern().confirmCloseConcern();

            caseMangementPage.getCloseCaseBtn().click();
            caseMangementPage.withRationaleForClosure("Closing").getCloseCaseBtn().click();

            Logger.Log("Viewing case is closed");
			homePage.getClosedCasesBtn().click();

			Logger.Log("Checking accessibility on closed case");
			cy.excuteAccessibilityTests();

			closedCasePage.getClosedCase(caseId).click();

            viewClosedCasePage.hasManagedBy("Empty", "Midlands and West - West Midlands");
        });
    });
});