import "cypress-localstorage-commands";
import "cypress-axe";
import caseApi from "../api/caseApi";
import concernsApi from "../api/concernsApi";
import { AuthenticationInterceptor } from "../auth/authenticationInterceptor";
import { Logger } from "../common/logger";
import { AuthenticationComponent } from "../auth/authenticationComponent";
import { CaseBuilder } from "cypress/api/caseBuilder";

Cypress.Commands.add("getByTestId", (id) => {
	cy.get(`[data-testid="${id}"]`);
});

Cypress.Commands.add("containsByTestId", (id) => {
	cy.get(`[data-testid*="${id}"]`);
});

Cypress.Commands.add("getById", (id) => {
	cy.get(`[id="${id}"]`);
});

Cypress.Commands.add("waitForJavascript", () => {
	// Need to look into this later
	// Essentially javascript validation is too slow and blocks submission even though the error has been corrected
	// Might be a more intelligent way to do this in the future
	cy.wait(1000);
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

//This line to excute accessibility, please make sure to add the link for the page you would like to test on accessibilitiesTestPages.json file.
Cypress.Commands.add("excuteAccessibilityTests", () => {
	Logger.Log("Executing the command");
	const continueOnFail = false;
	Logger.Log("Inject Axe");
	cy.injectAxe();

	Logger.Log("Checking accessibility");
	cy.checkA11y(
		undefined,
		{
			// These will be fixed one by one
			rules: {
				// "aria-allowed-role": { enabled: false },
				region: { enabled: false },
				label: { enabled: false }, // Create case failed
				listitem: { enabled: false }, // homepage
				"page-has-heading-one": { enabled: false }, // homepage
				"aria-input-field-name": { enabled: false }, // reassign
				"aria-required-children": { enabled: false }, // reassign
				"label-title-only": { enabled: false }, // reassign
				"aria-allowed-attr": { enabled: false }, // smoke
				"duplicate-id-aria": { enabled: false }, // smoke
				"empty-table-header": { enabled: false }, // smoke
				"color-contrast": { enabled: false }, // decisions
				"empty-heading": { enabled: false }, // srma
				// "duplicate-id-active": { enabled: false }, // nti
			},
		},
		undefined,
		continueOnFail
	);

	Logger.Log("Command finished");
});

Cypress.Commands.add("basicCreateCase", () => {
	caseApi.post(CaseBuilder.buildOpenCase()).then((caseResponse) => {
		const caseId = caseResponse.urn;
		concernsApi.post(caseId);

		cy.visit(`/case/${caseId}/management`);
		cy.reload();

		return cy.wrap(caseResponse.urn);
	});
});
