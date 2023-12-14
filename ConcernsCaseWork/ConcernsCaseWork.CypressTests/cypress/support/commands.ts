import "cypress-localstorage-commands";
import "cypress-axe";
import caseApi from "../api/caseApi";
import concernsApi from "../api/concernsApi";
import { AuthenticationInterceptor } from "../auth/authenticationInterceptor";
import { Logger } from "../common/logger";
import { AuthenticationComponent } from "../auth/authenticationComponent";
import { CaseBuilder } from "cypress/api/caseBuilder";
import caseMangementPage from "cypress/pages/caseMangementPage";
import { CreateCaseRequest } from "cypress/api/apiDomain";

Cypress.Commands.add("getByTestId", (id) => {
	cy.get(`[data-testid="${id}"]`);
});

Cypress.Commands.add("containsByTestId", (id) => {
	cy.get(`[data-testid*="${id}"]`);
});

Cypress.Commands.add("getById", (id) => {
	cy.get(`[id="${id}"]`);
});

Cypress.Commands.add("login", (params) => {
	cy.clearCookies();
	cy.clearLocalStorage();

	// Intercept all browser requests and add our special auth header
	// Means we don't have to use azure to authenticate
	new AuthenticationInterceptor().register(params);

	// Old method of using azure to login
	//const username = Cypress.env("username");
	//const password = Cypress.env("password");
	//new AuthenticationComponent().login(username, password);

	cy.visit("/");
});

Cypress.Commands.add("loginWithCredentials", () => {
	cy.clearCookies();
	cy.clearLocalStorage();

	// Used for prod as we cannot override login
	const username = Cypress.env("username");
	const password = Cypress.env("password");
	new AuthenticationComponent().login(username, password);

	cy.visit("/");
});

Cypress.Commands.add("excuteAccessibilityTests", () => {
	Logger.log("Executing the command");
	const continueOnFail = false;
	Logger.log("Inject Axe");
	cy.injectAxe();

	Logger.log("Checking accessibility");
	cy.checkA11y(
		undefined,
		{
			// These will be fixed one by one
			rules: {
				region: { enabled: false },
				"color-contrast": { enabled: false }, // decisions
			},
		},
		undefined,
		continueOnFail
	);

	Logger.log("Command finished");
});

Cypress.Commands.add("createNonConcernsCase", (request?: CreateCaseRequest) =>
{
	if (request == null) {
		request = CaseBuilder.buildOpenCase();
	}

	caseApi.post(request).then((caseResponse) => {
		const caseId = caseResponse.urn;

		cy.visit(`/case/${caseId}/management`);
		cy.reload();

		return cy.wrap(caseResponse);
	});
});

Cypress.Commands.add("basicCreateCase", (request?: CreateCaseRequest) => {

	if (request == null) {
		request = CaseBuilder.buildOpenCase();
	}

	caseApi.post(request).then((caseResponse) => {
		const caseId = caseResponse.urn;
		concernsApi.post(caseId);

		cy.visit(`/case/${caseId}/management`);
		cy.reload();

		return cy.wrap(caseResponse);
	});
});

Cypress.Commands.add("closeCase", () =>
{
	caseMangementPage.getCloseCaseBtn().click();
	caseMangementPage.withRationaleForClosure("Closing").getCloseCaseBtn().click();
});
