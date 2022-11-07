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
import './commands'
import './utils'

declare global {
    namespace Cypress {
        interface Chainable {
            getByTestId(id: string): Chainable<Element>;
            login(): Chainable<Element>;
            storeSessionData(): Chainable<Element>;
            selectMoR(): Chainable<Element>;
            createCase(): Chainable<Element>;
            randomSelectTrust(): Chainable<Element>;
            selectConcernType(): Chainable<Element>;
            addActionItemToCase(): Chainable<Element>;
            selectRiskToTrust(): Chainable<Element>;
            enterConcernDetails(): Chainable<Element>;
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
        }
    }
}