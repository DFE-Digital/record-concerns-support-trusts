/// <reference types ='Cypress'/>
import accessibilitiesTestPages from '../../../fixtures/accessibilitiesTestPages.json'


	describe('Check accessibility of the different pages', function () {
		beforeEach(() => {
			////cy.login();
cy.visit("/");
            cy.visit("/");
		});
        accessibilitiesTestPages.forEach((link) => {
         it(`Validate accessibility on ${link}`, function () {
            let url = Cypress.env('url')
            cy.visit(url)
            cy.visit(url+link)
            cy.excuteAccessibilityTests()
            })
        })
    })
