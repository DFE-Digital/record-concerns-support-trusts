import { Logger } from "../../common/logger";

describe("Home page tests", () => {
	before(() => {
		//cy.login();
		cy.visit("/");
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("Should be able to navigate back and forth from the home page", () =>
	{
		Logger.Log("Create, Find and Closed Case buttons should be displayed");
		cy.get('.govuk-button[href="/case/closed"]').should("be.visible");
		cy.get('.govuk-button[href="/trust"]').should("be.visible");
		cy.get('.govuk-button[href="/case"]').should("be.visible");

		Logger.Log("User clicks on Create Case and should see Search Trusts");
		cy.get('[href="/case"]').click();
		cy.get('#search').should('be.visible');

		Logger.Log("User clicks Back and should be taken back to the Active Casework screen");
		cy.getByTestId("cancel-trust-search").click();
		cy.get('[href="/case"').should('be.visible');
		cy.get('[href="/trust"').should('be.visible');
		cy.get('[href="/case/closed"').should('be.visible');

		Logger.Log("User clicks on Find Trust and should see Search Trusts");
		cy.get('[href="/trust"]').click();
		cy.get('#search').should('be.visible');

		Logger.Log("User clicks Back and should be taken back to the Active Casework screen");
		cy.getByTestId("cancel-trust-search").click();
		cy.get('[href="/case"').should('be.visible');
		cy.get('[href="/trust"').should('be.visible');
		cy.get('[href="/case/closed"').should('be.visible');
	});
});