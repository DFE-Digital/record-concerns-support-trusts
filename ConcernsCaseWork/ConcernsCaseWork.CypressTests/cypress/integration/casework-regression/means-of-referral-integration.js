import CaseManagementPage from "/cypress/pages/caseMangementPage";
import CreateCaseDetailsPage from "/cypress/pages/createCase/createCaseDetailsPage"
import { LogTask } from "../../support/constants";
import concernsApi from "cypress/api/concernsApi";

let apiKey = Cypress.env('apiKey');
let api = Cypress.env('api');
var caseid = "null";


describe("The correct items are visible on the details page", () => {
    before(() => {
        cy.login();
    });

    afterEach(() => {
        cy.storeSessionData();
    });

    it("Should show the correct information on the means of referral", () => {
        cy.get('[href="/case"]').click();
        cy.get("#search").should("be.visible");

        cy.randomSelectTrust();
        cy.get("#search__option--0").click();
        cy.getById("continue").click();

        cy.task(LogTask, "Select concern type (Financial: Deficit)")
        cy.get(".govuk-summary-list__value").then(($el) => {
        });
        cy.selectConcernType();

        cy.task(LogTask, "Setting the Overall Risk and Means of Referral and selecting Territory");
        cy.selectRiskToTrust();
        cy.selectTerritory();
        cy.get(".govuk-summary-list__value").then(($el) => {
        });
        cy.validateCreateCaseDetailsComponent();

        CreateCaseDetailsPage.setIssue();

        cy.task(LogTask, "navigating user to the homepage")
        cy.get('button[data-prevent-double-click^="true"]')
            .scrollIntoView().click();

        cy.task(LogTask, "GET Means of Referral by Case ID");
        CaseManagementPage.getCaseIDText().then((caseId) => {

            concernsApi.get(caseId)
                .then(response => {
                    expect(response[0].meansOfReferralId).to.eq(1);
                });
        });
    });
});
