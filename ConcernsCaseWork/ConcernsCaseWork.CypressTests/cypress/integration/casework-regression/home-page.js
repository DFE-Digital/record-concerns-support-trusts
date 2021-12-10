describe("Home page tests", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("Create, Find and Closed Case buttons should be displayed", () => {
		cy.get('.govuk-button[href="/case/closed"]').should("be.visible");
		cy.get('.govuk-button[href="/trust"]').should("be.visible");
		cy.get('.govuk-button[href="/case"]').should("be.visible");
	});

	it("user sorts by Primary Case Type", () => {
		 cy.get('.govuk-table [data-index="3"]').click()
		 cy.get('.govuk-table [data-index="3"]').click()

          let optionsArray = []
          let optionsArraySorted = []
          cy.log(optionsArray)
           cy.get('.govuk-table__row.tr__large td:nth-of-type(4)').each(($el) => {
               optionsArray.push($el.text().replaceAll('\t','').replaceAll('\n','').trim())
               optionsArraySorted.push($el.text().replaceAll('\t','').replaceAll('\n','').trim())
           })
            .then(() => {
                expect(optionsArray).to.deep.equal(optionsArraySorted.sort())
         })
        cy.log(optionsArray==optionsArraySorted)
	});

	it("user sorts by Trust/Academy", () => {
		cy.get('.govuk-table [data-index="2"]').click()

		 let optionsArray = []
		 let optionsArraySorted = []
		 cy.log(optionsArray)
		  cy.get('.govuk-table__row.tr__large td:nth-of-type(3)').each(($el) => {
			  optionsArray.push($el.text().replaceAll('\t','').replaceAll('\n','').trim())
			  optionsArraySorted.push($el.text().replaceAll('\t','').replaceAll('\n','').trim())
		  })
		   .then(() => {
			   expect(optionsArray).to.deep.equal(optionsArraySorted.sort())
		})
	   cy.log(optionsArray==optionsArraySorted)
   });

   it('User clicks on Create Case and should see Search Trusts', () => {
	   cy.get('[href="/case"]').click()
	   cy.get('#search').should('be.visible')
   });

   it('User clicks Back  and should be taken back to the Active Casework screen', () => {
    cy.get('#back-link-event').click()
	cy.get('[href="/case"').should('be.visible')
	cy.get('[href="/trust"').should('be.visible')
	cy.get('[href="/case/closed"').should('be.visible')
   });

   it('User clicks on Find Trust and should see Search Trusts', () => {
	cy.get('[href="/trust"]').click()
	cy.get('#search').should('be.visible')
});

it('User clicks Back  and should be taken back to the Active Casework screen', () => {
    cy.get('#back-link-event').click()
	cy.get('[href="/case"').should('be.visible')
	cy.get('[href="/trust"').should('be.visible')
	cy.get('[href="/case/closed"').should('be.visible')
   });
});


after(function () {
	cy.clearLocalStorage();
	cy.clearCookies();
});