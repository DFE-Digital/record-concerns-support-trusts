import createCaseTR from "/cypress/pages/createCase/createCaseTrustRiskPage";
import utils from "/cypress/support/utils"
import endpoints from "/cypress/support/endpoints"

let apiKey = Cypress.env('apiKey');
let url = Cypress.env('url');
var savedURN;
var savedOutgoingTrustUkprn;
var MoRurn;
var caseid
var originalAcademyPerformanceAdditionalInformation;

describe("The correct items are visible on the details page", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});


	it('GET Means of Referral by Case ID', () => {
        cy.request({
            method : 'GET',
            failOnStatusCode: false,
            url: url+"concerns-records/case/urn/"+caseid,
            headers: {
                ApiKey: apiKey,
                "Content-type" : "application/json"
            }
        })
        .then((response) =>{
            expect(response.status).to.eq(200);
            savedURN = response.body[0].urn;
            cy.log("URN = "+MoRurn);
            //savedOutgoingTrustUkprn = response.body[0].outgoingTrustUkprn;
            //cy.log("1st Project outgoingTrustUkprn = "+savedOutgoingTrustUkprn);
        })
        
    });


	const searchTerm =
		"Accrington St Christopher's Church Of England High School";

	it("User clicks on Create Case and should see Search Trusts", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
	});

	it("User searches for a valid Trust and selects it", () => {
		cy.randomSelectTrust();
		cy.get("#search__option--0").click();
	});

	it("Should allow a user to select a concern type (Financial: Deficit)", () => {
		cy.get(".govuk-summary-list__value").then(($el) =>{
			expect($el.text()).to.match(/(school|england|academy|trust)/i)
		});
		cy.selectConcernType();
	});

	it("Should allow a user to select the risk to the trust", () => {
		cy.selectRiskToTrust();
	});

	it("Not selecting a Means of Referral results in error", () => {

			createCaseTR.getConfirmCaseRatingButton();
			utils.getGovErrorSummaryList().should('exist');

	});

	it("Should allow a user to select a Means of Referral", () => {

		//cy.reload(true); may be needed to reset gov error
		cy.log("setStatusSelect ").then(() => {
			cy.log(createCaseTR.setMeansOfReferral("0") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log("logging the result "+stText)
				});
				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging returnedVal "+returnedVal)
				});
			});

			createCaseTR.getConfirmCaseRatingButton();
			utils.getGovErrorSummaryList().should('not.exist');

	});

	it("Should validate the create-case details component", () => {
		//Note 2 – Once means of referral is collected 
		//it isn’t resurfaced back to the caseworker.
		// It’s only collected for reporting from the back end.
		
		//Have to test thois using a get request
		// to validate the information was successfully sent

		endpoints.GetMeansOfReferral(endpoints.getCurrentCase())
	});


	it("Should validate the create-case details component", () => {
		cy.get(".govuk-summary-list__value").then(($el) =>{
			expect($el.text()).to.match(/(school|england|academy|trust)/i)
		});
		cy.validateCreateCaseDetailsComponent();
	});

	it("Should validate the initial details components", () => {
		cy.validateCreateCaseInitialDetails();
	});

	it("Should navigate user to the homepage on Cancel click ", () => {
		cy.get('a[data-prevent-double-click^="true"]')// REPLACE WITH CONTUNUE CTA
			.scrollIntoView().click();
		cy.get('caption[class="govuk-table__caption govuk-table__caption--m"]').then(($actcase) =>{
            expect($actcase).to.be.visible
            expect($actcase.text()).to.match(/(active|casework)/i)
		});
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
