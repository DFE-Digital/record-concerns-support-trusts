const accessibilitiesTestPages = require('../../../fixtures/accessibilitiesTestPages.json');
import { Logger } from "cypress/common/logger";

describe.only('Check accessibility of the different pages', function () {
    beforeEach(() => {
        Logger.Log("Logging in");
        cy.task("log", accessibilitiesTestPages);
        cy.login();
    });

    accessibilitiesTestPages.forEach((link) => {
        it(`Validate accessibility on ${link}`, () =>
        {
            cy.visit(link);
            cy.excuteAccessibilityTests();
        });
    })
})
