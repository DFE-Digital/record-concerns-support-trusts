import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import srmaAddPage from "/cypress/pages/caseActions/srmaAddPage";
import { LogTask } from "../../../support/constants";

describe("User can add case actions to an existing case", () => {
    before(() => {
        cy.login();
    });

    afterEach(() => {
        cy.storeSessionData();
    });

    it("Should add a case action to an existing case", () => {
        cy.checkForExistingCase();

        cy.task(LogTask, "Option exists for adding SRMA Case Action");
        cy.reload();
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Srma')
        AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);

        cy.task(LogTask, "Move to the correct Case Action page");
        cy.get('button[data-prevent-double-click*="true"]').click();

        cy.wait(500).then(() => {
            const err = Cypress.$('.govuk-list.govuk-error-summary__list');
            cy.log(err.length);
        });

        srmaAddPage.getHeadingText().should('contain.text', "School Resource Management Adviser (SRMA)");

        cy.task(LogTask, "Cannot proceed without valid status");
        srmaAddPage.setDateOffered();
        srmaAddPage.getAddCaseActionBtn().click();
        cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Select status');
        cy.reload();

        cy.task(LogTask, "Cannot proceed without valid date");
        srmaAddPage.setStatusSelect(0);
        cy.get('[id="dtr-day"]').type("0");
        cy.get('[id="dtr-month"]').type("0");
        cy.get('[id="dtr-year"]').type("0");
        srmaAddPage.getAddCaseActionBtn().click();
        cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the date SRMA was offered to the trust');
        cy.reload();
    });
});
