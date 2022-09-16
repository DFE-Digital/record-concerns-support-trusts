import CaseManagementPage from "/cypress/pages/caseMangementPage";
import CreateCaseDetailsPage from "/cypress/pages/createCase/createCaseDetailsPage"

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
		});
		cy.selectConcernType();
	});

	it("Should allow a user to set the Overall Risk and Means of Referral", () => {
		cy.selectRiskToTrust();
	});

	it("Should validate the create-case details component", () => {
		cy.get(".govuk-summary-list__value").then(($el) =>{
		});
		cy.validateCreateCaseDetailsComponent();
	});

	it("Should set the case issue", () => {
		CreateCaseDetailsPage.setIssue();
	});

	it("Should navigate user to the homepage on Create click ", () => {
		cy.get('button[data-prevent-double-click^="true"]')
			.scrollIntoView().click();
	});

	it('GET Means of Referral by Case ID', function () {


		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseManagementPage.getCaseIDText() ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					caseid  = returnedVal;
					cy.log("logging the result "+caseid)
				});
				cy.log(self.stText);
				caseid  = returnedVal;
				cy.log("logging returnedVal "+returnedVal)
				});
			});


        cy.request({
            method : 'GET',
            failOnStatusCode: false,
            url: api+"/v2/concerns-records/case/urn/"+caseid,
            headers: {
                ApiKey: apiKey,
                "Content-type" : "application/json"
            }
        })
        .then((response) =>{
            expect(response.status).to.eq(200);
			expect(response.body.data[0].meansOfReferralUrn).to.eq(12574);
        })  
   });

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
