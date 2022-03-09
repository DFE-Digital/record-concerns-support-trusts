describe("User closes a case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm = "10058682"
		//"Accrington St Christopher's Church Of England High School";

	it("User creates a case", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
		cy.get("#search").type(searchTerm + "{enter}");
		cy.get("#search__option--0").click();
		cy.selectConcernType();
		cy.selectRiskToTrust();
		cy.enterConcernDetails();
	});

	//Opens the first active case in the list
	it("Opens an active case", () => {
		cy.get('.buttons-topOfPage [href="/"]').click();
		//Stores the ID of the case...
		cy.get('#your-casework tr:nth-child(1)  td:nth-child(1)  a').then(($el) => {
			cy.wrap($el.text()).as("closedCaseId");
		});
		cy.get('#your-casework tr:nth-child(1) td:nth-child(1) a').click();
	});

	it("User can close an open concern", function () {
		cy.closeAllOpenConcerns();
	});

	it("User can close an open case", function () {
		cy.get(
			'[href*="/case/' + this.closedCaseId + '/management/closure"]'
		).click();
		cy.get("#case-outcomes").type("SAMPLE CLOSURE TEXT");
		cy.get(".govuk-button").click();
	});

	it("Case should be marked as closed and removed from active cases", function () {
		cy.get("#main-content tr:nth-child(1) td:nth-child(1) a").then(($el) => {
			cy.wrap($el.text()).as("caseIdAfter");
			//Checks the the Case ID is no longer listed as Active
			expect(this.closedCaseId).to.not.equal($el.text());
		});
		cy.get('[href*="/case/closed"]').click();
		//Checks the case ID is listed as closed
		cy.get("#main-content tr:nth-child(1) td:nth-child(1)").should('contain', this.closedCaseId);
	});

	it("The Trust page should contain a closed cases table", () => {
		cy.visitPage('/trust')
		//cy.visit(Cypress.env('url')+'/trust')
		cy.get("#search").type(searchTerm + "{enter}");
		cy.get("#search__option--0").click();			
		cy.get('table:nth-child(6) > caption').scrollIntoView();
		cy.get('table:nth-child(6) > caption').should(($titl) => {
			expect($titl.text().trim()).equal("Closed cases");
		});
	});

	it("Case should be visible in the Trust page under closed cases", function () {
		cy.get('table:nth-child(6) > tbody').children().should('contain', this.closedCaseId);
	});

	it("User is taken to the Case Page when clicking the closed case id", function () {
		cy.get('table:nth-child(6) > tbody').children().contains(this.closedCaseId).click()
		cy.get('[id="tab_trust-overview"]').click();
	});

	it("Case should be visible in the Case Page Trust Tab under closed cases", function () {				
		cy.get('table:nth-child(4) > caption').scrollIntoView();
		cy.get('table:nth-child(4) > caption').should(($titl) => {
			expect($titl.text().trim()).equal("Closed cases")
		});
		cy.get('table:nth-child(4) > tbody').children().should('contain', this.closedCaseId)
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
		});
	});
