describe("User can add action srma to existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm =
		"Accrington St Christopher's Church Of England High School";

	it("User enters the case page case", () => {
		cy.checkForExistingCase(true);
	});

	it("User on the Case page is warned when proceeding without selecting an action", () => {
		cy.get('[class="govuk-button"][role="button"]').click();

		cy.get('button[data-prevent-double-click^="true"]').then(($btn) =>{
			expect($btn.text()).to.match(/(Add to case)/i);
		cy.get('button[data-prevent-double-click^="true"]').click()
			
	  	 })

		   cy.get('[class="govuk-list govuk-error-summary__list"]')
		   		.should('contain.text', 'Please select an action to add');
			//Cleanup to remove any lingering validation code for next steps
			cy.reload();
		});

	//Currently failing due to existing bug:
	it("User can Cancel and is returned to the Case ID Page", () => {
		//cy.addActionItemToCase();
		cy.get('[id="cancel-link-event"]').click();

		cy.get('[class="govuk-caption-m"]').then(($heading) =>{
			expect($heading).to.be.visible
			expect($heading.text()).to.match(/(Case ID)/i);
		});
	});


	it("User has option to add an action item to case", () => {
		cy.get('[class="govuk-button"][role="button"]').click();
		cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
	});

	it("User clicking add to case is taken to the action page", () => {
		cy.get('button[data-prevent-double-click*="true"]').click()
	});

	it("User chooses an SRMA status", () => {

	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
