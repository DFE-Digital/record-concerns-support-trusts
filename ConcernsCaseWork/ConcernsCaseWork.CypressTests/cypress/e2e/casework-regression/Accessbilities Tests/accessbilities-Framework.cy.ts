const accessibilitiesTestPages = require('../../../fixtures/accessibilitiesTestPages.json');

describe('Check accessibility of the different pages', function () {
    beforeEach(() => {
        cy.login();
    });
    accessibilitiesTestPages.forEach((link) => {
        it(`Validate accessibility on ${link}`, () =>
        {
            cy.visit(link)
            cy.excuteAccessibilityTests()
        });
    })
})
