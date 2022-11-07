import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import { LogTask } from "../../../support/constants";

describe("User can resolve an SRMA and is given validation based on options chosen", () => {
    before(() => {
        cy.login();
    });

    afterEach(() => {
        cy.storeSessionData();
    });

    it("Should validate an SRMA", () => {
        cy.task(LogTask, "User creates a case");
        cy.visit(Cypress.env('url'), { timeout: 30000 });
        cy.checkForExistingCase(true);
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Srma');
        AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);
        AddToCasePage.getAddToCaseBtn().click();

        cy.task(LogTask, "User can set SRMA status to  Trust Considering");
        cy.get('[id*="status"]').eq(0).click();
        cy.get('label.govuk-label.govuk-radios__label').eq(0).invoke('text').then(term => {
            const stText = term.trim();

            cy.task(LogTask, "User can enter a valid date");
            cy.get('[id="dtr-day"]').type("10");
            cy.get('[id="dtr-month"]').type("12");
            cy.get('[id="dtr-year"]').type("2022");

            cy.task(LogTask, "User can successfully add SRMA to a case");
            cy.get('[id="add-srma-button"]').click();

            cy.task(LogTask, "User can click on a link to view a live SRMA record");
            CaseManagementPage.getOpenActionsTable().children().should(($srma) => {
                expect($srma).to.be.visible
                expect($srma.text()).to.contain("SRMA");
            })

            CaseManagementPage.getOpenActionsTable().should(($status) => {
                expect($status).to.be.visible
                expect($status.text().trim()).to.contain(stText);
            })
        });

        CaseManagementPage.getOpenActionLink("srma").click();

        cy.task(LogTask, "User can set status to Trust Considering");
        cy.get('[class="govuk-link"]').eq(0).click();

        cy.get('[id*="status"]').eq(0).click();
        cy.get('label.govuk-label.govuk-radios__label').eq(0).invoke('text').then(term => {
            cy.wrap(term.trim()).as("stText");
            cy.log("Status set as " + term);
            cy.get('[id="add-srma-button"]').click();
            cy.get('[class="govuk-table__row"]').should(($row) => {
                expect($row.eq(0).text().trim()).to.contain("Trust considering").and.to.match(/Status/i);
            });
        })

        cy.task(LogTask, "User on the SRMA page cannot decline SRMA without reason populated")
        cy.get('[id="complete-decline-srma-button"]').click();

        //Tests that there is error validation to force entry of both dates
        let err = '[class="govuk-list govuk-error-summary__list"]';
        cy.log((err).length);

        if ((err).length > 0) {

            cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible')
            cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
        } else {
            //this code path is a fallback in case the data is altered mid test
            cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
        }

        cy.reload();

        cy.task(LogTask, "User can set status to preparing for deployment");
        cy.get('[class="govuk-link"]').eq(0).click();
        cy.get('[id*="status"]').eq(1).click();
        cy.get('label.govuk-label.govuk-radios__label').eq(1).invoke('text').then(term => {
            cy.wrap(term.trim()).as("stText");
            cy.log("Status set as " + term);
            cy.get('[id="add-srma-button"]').click();
            cy.get('[class="govuk-table__row"]').should(($row) => {
                expect($row.eq(0).text().trim()).to.contain(term.trim()).and.to.match(/Status/i);
            });
        });

        cy.task(LogTask, "User on the SRMA page cannot decline SRMA without reason populated");
        cy.get('[id="complete-decline-srma-button"]').click();

        //Tests that there is error validation to force entry of both dates
        err = '[class="govuk-list govuk-error-summary__list"]';
        cy.log((err).length);

        if ((err).length > 0) {

            cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible')
            cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
        } else {
            //this code path is a fallback in case the data is altered mid test
            cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
        }

        cy.reload();

        cy.task(LogTask, "User on the SRMA page cannot cancel SRMA without reason populated");
        cy.get('[href*="/cancel"]').click();

        //Tests that there is error validation to force entry of both dates
        err = '[class="govuk-list govuk-error-summary__list"]';
        cy.log((err).length);

        if ((err).length > 0) {

            cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible')
            cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
        } else {
            //this code path is a fallback in case the data is altered mid test
            cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
        }
        cy.reload();

        cy.task(LogTask, "User CAN cancel with reason populated");
        cy.get('[class="govuk-link"]').eq(2).click();

        let rand = Math.floor(Math.random() * 2)

        cy.get('[id^="reason"]').eq(rand).click();
        cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
            cy.wrap(term.trim()).as("stText");
            cy.log("Reason set as " + term);
            cy.get('[id="add-srma-button"]').click();
            cy.get('[class="govuk-table__row"]').should(($row) => {
                expect($row.eq(2).text().trim()).to.contain(term.trim()).and.to.match(/Reason/i);
                expect($row.eq(0).text().trim()).to.contain("Preparing for deployment").and.to.match(/Status/i);
            });
        });

        cy.task(LogTask, "User CAN cancel with reason populated");
        cy.get('[class="govuk-link"]').eq(2).click();
    });

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    });

});
