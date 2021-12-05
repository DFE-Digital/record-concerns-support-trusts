describe("Create a brand new case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("Should display the Concerns Casework Dashboard", () => {
		cy.get(".govuk-table__caption").should(
			"contain.text",
			"Your active casework"
		);
	});

	it("Should allow the user to set the primary concern type", () => {
		cy.get('*[href="/case"]').click();
		cy.get("#search").type("Church Hill Church Of England Junior School{enter}");
		cy.get("#search__option--0").click();
        cy.get('.govuk-radios__item label').contains('Compliance').click()
        cy.get('#sub-type-1').click()
        cy.get('[value="398:Amber-Green"]').click()
        cy.get('.govuk-button').click()
	});

    it('Should allow the user to enter initial case details', () => {
        let date = new Date();
        let dateText = date.toTimeString()
        cy.get('#issue').type('Issue data entered at: '+dateText)
        cy.get('#current-status').type('Current Status data entered at: '+dateText)
        cy.get('#case-aim').type('Case Aim data entered at: '+dateText)
        cy.get('#de-escalation-point').type('De-escalation Point data entered at: '+dateText)
        cy.get('#next-steps').type('Next Steps data entered at: '+dateText)
        cy.get('.govuk-button').click()
    });

    it('Should inform the user that a Concern has been successfully created', () => {
        cy.get('.moj-timeline__description p').should('contain.text','Concern created')
    });
});

after(function () {
    cy.clearLocalStorage();
    cy.clearCookies()
});