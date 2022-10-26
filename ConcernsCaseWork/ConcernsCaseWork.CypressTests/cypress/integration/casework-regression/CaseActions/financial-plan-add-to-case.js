import { LogTask } from "../../../support/constants";
import utils from "../../../support/utils";
import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import FinancialPlanPage from "/cypress/pages/caseActions/financialPlanPage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";

describe("User can add Financial Plan case action to an existing case", () => {
    before(() => {
        cy.login();
    });

    afterEach(() => {
        cy.storeSessionData();
    });

    it("Should add a financial plan to an existing case", () => {
        cy.checkForExistingCase();

        cy.task(LogTask, "Has option to add Financial Plan Case Action to a case")
        cy.reload();
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('FinancialPlan')
        AddToCasePage.getCaseActionRadio('FinancialPlan').siblings().should('contain.text', AddToCasePage.actionOptions[2]);

        validatePage();

        cy.task(LogTask, "Navigates to correct Case Action page");
        FinancialPlanPage.getHeadingText().then(term => {
            expect(term.text().trim()).to.match(/(Financial Plan)/i);
        });

        cy.task(LogTask, "Error is shown when entering invalid date");
        FinancialPlanPage.getDatePlanRequestedDay().type(Math.floor(Math.random() * 21) + 10);
        FinancialPlanPage.getDatePlanRequestedMon().type(Math.floor(Math.random() * 3) + 10);
        FinancialPlanPage.getDatePlanRequestedYear().type("3022");
        FinancialPlanPage.getUpdateBtn().click();
        utils.validateGovErorrList('invalid date');
        cy.reload(true);

        cy.task(LogTask, "Can enter valid date");
        FinancialPlanPage.getDatePlanRequestedDay().type(Math.floor(Math.random() * 21) + 10).invoke('val').then(dtrday => {
            cy.wrap(dtrday.trim()).as("day");
        });

        FinancialPlanPage.getDatePlanRequestedMon().type(Math.floor(Math.random() * 3) + 10).invoke('val').then(dtrmon => {
            cy.wrap(dtrmon.trim()).as("month");
        });

        FinancialPlanPage.getDatePlanRequestedYear().type("2022").invoke('val').then(dtryr => {
            cy.wrap(dtryr.trim()).as("year");
        });

        FinancialPlanPage.getDatePlanReceivedDay().type(Math.floor(Math.random() * 21) + 10);
        FinancialPlanPage.getDatePlanReceivedMon().type(Math.floor(Math.random() * 3) + 10);
        FinancialPlanPage.getDatePlanReceivedYear().type("2022");

        cy.task(LogTask, "Can add financial plan to case");
        FinancialPlanPage.getUpdateBtn().click();
    });

    function validatePage() {
        cy.task(LogTask, "Show validation when Case Action already exists");
    
        AddToCasePage.getAddToCaseBtn().click();
    
        cy.wait(2000);
    
        const err = Cypress.$('.govuk-list.govuk-error-summary__list');
        cy.log("err.length " + err.length);
    
        if (err.length > 0) { //Cypress.$ needed to handle element missing exception
            cy.log("Case Action already exists");
    
    
            cy.log(utils.checkForGovErrorSummaryList());
    
            if (utils.checkForGovErrorSummaryList() > 0) {
                cy.log("Case Action already exists");
    
                cy.visit(Cypress.env('url'), { timeout: 30000 });
                cy.checkForExistingCase(true);
                CaseManagementPage.getAddToCaseBtn().click();
                AddToCasePage.addToCase('FinancialPlan');
                AddToCasePage.getCaseActionRadio('FinancialPlan').siblings().should('contain.text', AddToCasePage.actionOptions[2]);
    
                AddToCasePage.getAddToCaseBtn();
                FinancialPlanPage.getHeadingText().then(term => {
                    expect(term.text().trim()).to.match(/(Financial plan)/i);
                });
    
            } else {
                cy.log("No Case Action exists");
                FinancialPlanPage.getHeadingText().then(term => {
                    expect(term.text().trim()).to.match(/(Financial plan)/i);
                });
            }
    
            AddToCasePage.getAddToCaseBtn().click();
        } else {
            cy.log("No Case Action exists");
            cy.log(utils.checkForGovErrorSummaryList());
        }
    }
});

