import createCaseTR from "/cypress/pages/createCase/createCaseTrustRiskPage";

describe("The correct items are visible on the details page", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
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
		//	expect($el.text()).to.match(/(school|england|academy|trust)/i)
		});
		cy.selectConcernType();
	});

	it("Should allow a user to set the Overall Risk and Means of Referral", () => {
		cy.selectRiskToTrust();
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
/*
	it("Should navigate user to the homepage on Cancel click ", () => {
		cy.get('a[data-prevent-double-click^="true"]')
			.scrollIntoView().click();
		cy.get('caption[class="govuk-table__caption govuk-table__caption--m"]').then(($actcase) =>{
            expect($actcase).to.be.visible
            expect($actcase.text()).to.match(/(active|casework)/i)
		});
	});
*/
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
