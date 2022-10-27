import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import ntiAddPage from "/cypress/pages/caseActions/ntiAddPage";
import CaseActionsBasePage from "/cypress/pages/caseActions/caseActionsBasePage";
import homePage from "/cypress/pages/homePage.js";
import closedCasePage from "/cypress/pages/closedCasePage.js";

describe('User can view and navigate Closed cases', () => {
    before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

    it('User clicks Closed Case button and is taken to closed case page', () => {    
        homePage.getClosedCasesBtn().click();
        closedCasePage.getClosedCasesTable().should('be.visible');
    });

    it('User can view a closed case', () => {
        closedCasePage.getClosedCasesLink().first().should('be.visible');
        closedCasePage.getClosedCasesLink().first().click();
        CaseManagementPage.getCaseID().should('be.visible');
    });

    it('User clicks Back and should be taken back to the Closed Cases', () => {
        CaseManagementPage.getBackBtn().click();
        closedCasePage.getClosedCasesTable().should('be.visible');
    });
    
    it('User clicks Back and should be taken back to the Active Casework screen', () => {
        CaseManagementPage.getBackBtn().click();
        homePage.getHeadingText().should('be.visible');
        homePage.getClosedCasesBtn().should('be.visible');
    });
    

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    });
});