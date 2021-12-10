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
});


after(function () {
	cy.clearLocalStorage();
	cy.clearCookies();
});