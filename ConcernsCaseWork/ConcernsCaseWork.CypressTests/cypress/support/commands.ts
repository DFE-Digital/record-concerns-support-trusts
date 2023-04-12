import "cypress-localstorage-commands";
import "cypress-axe";
import caseApi from "../api/caseApi";
import concernsApi from "../api/concernsApi";
import { AuthenticationInterceptor } from "../auth/authenticationInterceptor";
//import {AuthenticationComponent} from  "../auth/authenticationComponent";

Cypress.Commands.add("getByTestId", (id) => {
	cy.get(`[data-testid="${id}"]`);
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

//This line to excute accessibility, please make sure to add the link for the page you would like to test on accessibilitiesTestPages.json file.
Cypress.Commands.add("excuteAccessibilityTests", () => {
	const wcagStandards = ["wcag22aa"];
	const impactLevel = ["critical", "minor", "moderate", "serious"];
	const continueOnFail = false;
	cy.injectAxe();
	cy.checkA11y(
		undefined,
		{
			runOnly: {
				type: "tag",
				values: wcagStandards,
			},
			includedImpacts: impactLevel,
		},
		undefined,
		continueOnFail
	);
});

Cypress.Commands.add("basicCreateCase", () => {
    caseApi.post()
    .then((caseResponse) => {
        const caseId = caseResponse.urn;
        concernsApi.post(caseId);

        cy.visit(`/case/${caseId}/management`);
        cy.reload();

		return cy.wrap(caseResponse.urn);
    });
});