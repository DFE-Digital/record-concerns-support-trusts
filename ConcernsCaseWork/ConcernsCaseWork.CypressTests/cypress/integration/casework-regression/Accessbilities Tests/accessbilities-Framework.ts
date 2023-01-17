/// <reference types ='Cypress'/>
import accessibilitiesTestPages from '../../../fixtures/accessibilitiesTestPages.json'
const wcagStandards = [ "wcag22aa"];
const impactLevel = ["critical", "minor", "moderate", "serious"];
const continueOnFail = false;

	describe('Check accessibility of the different pages', function () {
		beforeEach(() => {
			cy.login();
		});
        accessibilitiesTestPages.forEach((link) => {
         it('Validate accessibility on different pages', function () {
            let url = Cypress.env('url')
            cy.visit(url)
            cy.visit(url+link)
            cy.excuteAccessibilityTests()
            })
        })
    })
