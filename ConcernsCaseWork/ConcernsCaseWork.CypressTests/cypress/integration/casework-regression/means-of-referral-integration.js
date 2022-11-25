import CaseManagementPage from "/cypress/pages/caseMangementPage";
import CreateCaseDetailsPage from "/cypress/pages/createCase/createCaseDetailsPage"
import { LogTask } from "../../support/constants";

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

        cy.task(LogTask, "Select concern type (Financial: Deficit)")
        cy.get(".govuk-summary-list__value").then(($el) => {
        });
        cy.selectConcernType();

        cy.task(LogTask, "Setting the Overall Risk and Means of Referral");
        cy.selectRiskToTrust();

        cy.get(".govuk-summary-list__value").then(($el) => {
        });
        cy.validateCreateCaseDetailsComponent();

        CreateCaseDetailsPage.setIssue();

        cy.task(LogTask, "navigating user to the homepage")
        cy.get('button[data-prevent-double-click^="true"]')
            .scrollIntoView().click();

        cy.task(LogTask, "GET Means of Referral by Case ID");
        CaseManagementPage.getCaseIDText().then((returnedVal) => {

            cy.request({
                method: 'GET',
                failOnStatusCode: false,
                url: api + "/v2/concerns-records/case/urn/" + returnedVal,
                headers: {
                    ApiKey: apiKey,
                    "Content-type": "application/json"
                }
            })
            .then((response) => {
                expect(response.status).to.eq(200);
                expect(response.body.data[0].meansOfReferralId).to.eq(1);
            })
        });
    });
});
