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
import { CreateCaseRequest, CreateCaseResponse } from 'cypress/api/apiDomain';

declare global {
    namespace Cypress {
        interface Chainable {
            getByTestId(id: string): Chainable<Element>;
            containsByTestId(id: string): Chainable<Element>;
            getById(id: string): Chainable<Element>;
            login(params?: AuthenticationInterceptorParams): Chainable<Element>;
            loginWithCredentials(): Chainable<Element>;
			excuteAccessibilityTests(): Chainable<Element>;
            basicCreateCase(request?: CreateCaseRequest): Chainable<CreateCaseResponse>;
            createNonConcernsCase(request?: CreateCaseRequest): Chainable<CreateCaseResponse>;
            closeCase(): Chainable<Element>;
        }
    }
}