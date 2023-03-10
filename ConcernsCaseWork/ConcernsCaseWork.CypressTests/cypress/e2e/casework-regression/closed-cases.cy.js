import CaseManagementPage from "/cypress/pages/caseMangementPage";
import homePage from "cypress/pages/homePage";
import closedCasePage from "/cypress/pages/closedCasePage.js";
import { LogTask } from "../../support/constants";

describe('User can view and navigate Closed cases', () => {
    beforeEach(() => {
		cy.login();
	});

	it("Testing closed case functionality", () => {

        cy.task(LogTask, "User clicks Closed Case button and is taken to closed case page")
        homePage.getClosedCasesBtn().click();
        closedCasePage.getClosedCasesTable().should('be.visible');

        cy.task(LogTask, "User can view a closed case");
        closedCasePage.getClosedCasesLink().first().should('be.visible');
        closedCasePage.getClosedCasesLink().first().click();
        CaseManagementPage.getCaseID().should('be.visible');

        cy.task(LogTask, "User clicks Back and should be taken back to the Closed Cases");
        CaseManagementPage.getBackBtn().click();
        closedCasePage.getClosedCasesTable().should('be.visible');

        cy.task(LogTask, "User clicks Back and should be taken back to the Active Casework screen");
        CaseManagementPage.getBackBtn().click();
        cy.contains("Your active casework");
        homePage.getClosedCasesBtn().should('be.visible');
    });

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    });
});