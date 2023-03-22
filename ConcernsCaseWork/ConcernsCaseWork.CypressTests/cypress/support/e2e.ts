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
import { AuthenticationInterceptorParams } from 'cypress/auth/authenticationInterceptor';
import './commands'

declare global {
    namespace Cypress {
        interface Chainable {
            getByTestId(id: string): Chainable<Element>;
            getById(id: string): Chainable<Element>;
            waitForJavascript(): Chainable<Element>;
            login(params?: AuthenticationInterceptorParams): Chainable<Element>;
            selectMoR(): Chainable<Element>;
            createCase(): Chainable<Element>;
            randomSelectTrust(): Chainable<Element>;
            selectConcernType(): Chainable<Element>;
            selectRiskToTrust(): Chainable<Element>;
            selectTerritory(): Chainable<Element>;
            enterConcernDetails(): Chainable<Element>;
			excuteAccessibilityTests(): Chainable<Element>;
            selectConcern(expectedNumberOfRagStatus: number, ragStatus: string): Chainable<number>;
            validateCreateCaseDetailsComponent(): Chainable<Element>;
            validateCreateCaseInitialDetails(): Chainable<Element>;
            addConcernsDecisionsAddToCase(): Chainable<Element>;
            basicCreateCase(): Chainable<number>;
        }
    }

	
}