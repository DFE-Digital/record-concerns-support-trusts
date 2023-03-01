// ***********************************************************
// This example support/index.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import { CaseworkerClaim, EnvApiKey, EnvUsername } from 'cypress/constants/cypressConstants';
import { Logger } from '../common/logger';
import './commands'
import './utils'

declare global {
    namespace Cypress {
        interface Chainable {
            getByTestId(id: string): Chainable<Element>;
            getById(id: string): Chainable<Element>;
            waitForJavascript(): Chainable<Element>;
            login(): Chainable<Element>;
            storeSessionData(): Chainable<Element>;
            selectMoR(): Chainable<Element>;
            createCase(): Chainable<Element>;
            randomSelectTrust(): Chainable<Element>;
            selectConcernType(): Chainable<Element>;
            addActionItemToCase(): Chainable<Element>;
            selectRiskToTrust(): Chainable<Element>;
            selectTerritory(): Chainable<Element>;
            enterConcernDetails(): Chainable<Element>;
			excuteAccessibilityTests(): Chainable<Element>;
            visitPage(slug: string): Chainable<Element>;
            editRiskToTrust(cta: string, rag: string): Chainable<Element>;
            selectConcern(expectedNumberOfRagStatus: number, ragStatus: string): Chainable<number>;
            validateCreateCaseDetailsComponent(): Chainable<Element>;
            validateCreateCaseInitialDetails(): Chainable<Element>;
            validateCaseManagPage(): Chainable<Element>;
            closeAllOpenConcerns(): Chainable<Element>;
            closeSRMA(): Chainable<Element>;
            checkForExistingCase(force: boolean): Chainable<Element>;
            createSRMA(): Chainable<Element>;
            addConcernsDecisionsAddToCase(): Chainable<Element>;
            basicCreateCase(): Chainable<number>;
            checkForExistingCase(): Chainable<Element>;
        }
    }

	
}

before(() => {
	const url = Cypress.env('url');

	cy.intercept(
		{
			url: url + "/**",
			middleware: true
		},
		(req) =>
		{
			// Set an auth header on every request made by the browser
			req.headers['Authorization'] = `Bearer ec6e49d6-4f9a-498b-a1c3-ccac46d514e9`;
            req.headers = {
                ...req.headers,
                "ApiKey": Cypress.env(EnvApiKey),
                "Content-type": "application/json",
                "x-user-context-role-0" : CaseworkerClaim,
                "x-user-context-name" : Cypress.env(EnvUsername)
            }
		}
	)
})