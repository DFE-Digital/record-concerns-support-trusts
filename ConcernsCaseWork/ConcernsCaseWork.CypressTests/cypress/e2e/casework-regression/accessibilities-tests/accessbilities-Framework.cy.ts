const accessibilitiesTestPages = require('../../../fixtures/accessibilitiesTestPages.json');
import { Logger } from "cypress/common/logger";

describe('Check accessibility of the different pages', function () {
    beforeEach(() => {
        Logger.Log("Logging in");
        cy.login();
    });

    accessibilitiesTestPages.forEach((link) => {
        it(`Validate accessibility on ${link}`, () =>
        {
            Logger.Log(link);
            // cy.visit(link);
            // cy.excuteAccessibilityTests();
        });
    })
})
