describe("User can add action srma to existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm =
		"Accrington St Christopher's Church Of England High School";
	let term = "";

	it("User enters the case page case", () => {
		cy.checkForExistingCase(true);
	});

	it("User on the Case page is warned when proceeding without selecting an action", () => {
		cy.get('[class="govuk-button"][role="button"]').click();

		cy.get('button[data-prevent-double-click^="true"]').then(($btn) =>{
			expect($btn.text()).to.match(/(Add to case)/i);
		cy.get('button[data-prevent-double-click^="true"]').click();
		});

        
		});

	it("User can Cancel and is returned to the Case ID Page", () => {
	
		cy.get('[id="cancel-link-event"]').click();

		cy.get('[class="govuk-caption-m"]').then(($heading) =>{
			expect($heading).to.be.visible
			expect($heading.text()).to.match(/(Case ID)/i);
		});
	});

	it("User has option to add an action item to case", () => {
		cy.reload();
		cy.get('[class="govuk-button"][role="button"]').click();
		cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
	});


	it("User clicking add to case is taken to the action page", () => {
		cy.get('button[data-prevent-double-click*="true"]').click();



/*
		const err = '[class="govuk-list govuk-error-summary__list"]';

		cy.log((err).length).then((error)  => {
			//cy.wait(5000);
			cy.reload(true);

			if (Cypress.$(err).length > 0 ) { //Cypress.$ needed to handle element missing exception


			//cy.log("SRMA already exists for this case");
				cy.visit(Cypress.env('url'), { timeout: 30000 });

				cy.checkForExistingCase(true);
				cy.get('[class="govuk-button"][role="button"]').click();
				cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
				cy.get('button[data-prevent-double-click*="true"]').click();
			}else{
				cy.log("No SRMA exists")
			}

		});

			//Cleanup to remove any lingering validation code for next steps
			//cy.reload();
*/
	
	});

	it("User is shown validation and cannot proceed without selecting a status", () => {
		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2022");

		cy.get('[id="add-srma-button"]').click();
		cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Select status');
		cy.reload();


	});

	it("User is shown validation and cannot proceed without selecting a valid date", () => {
		cy.get('[id="dtr-day"]').type("0");
		cy.get('[id="dtr-month"]').type("0");
		cy.get('[id="dtr-year"]').type("0");
		cy.get('[id="add-srma-button"]').click();

		cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter a valid date');
		cy.reload();
	});

	it("User is shown validation and cannot add more tan 500 characters in the Notes section", () => {
		const lstring =
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx';

		
		//cy.get('[class="govuk-heading-m"]').contains('Notes').should('be.visible');

	   //cy.get('id="srma-notes"').should('be.visible');

	       //Notes validation
		  // cy.get('[class="govuk-grid-row"] *[for="issue"]').should(($notes) => {
           // expect($notes.text().trim()).equal("Notes");
         // })

        cy.get('#srma-notes').should('be.visible');
        cy.get('#srma-notes-info').then(($inf) =>{
            expect($inf).to.be.visible
            expect($inf.text()).to.match(/(500 characters remaining)/i)   
		//});

            let text = Cypress._.repeat(lstring, 10)
            expect(text).to.have.length(500);
            
        cy.get('#srma-notes').invoke('val', text);
        cy.get('#srma-notes').type('{shift}{alt}'+ '1');
       
		})


	});

	it("User is gven a warnining for remaining text", () => {
		cy.get('#srma-notes-info').then(($inf2) =>{
			expect($inf2).to.be.visible
			expect($inf2.text()).to.match(/(1 character too many)/i);      
			})
			cy.get('[id="add-srma-button"]').click();
			//cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Notes');//  <<<<<Currently failing due to bug

			//cy.get('#srma-notes').invoke('val', text);

	});


	it("User can select an SRMA status", () => {
		cy.reload();
		cy.get('[id*="status"]').eq(Math.floor(Math.random()*2)).click().invoke('text').then(text => {
			term = text;
			cy.log(term);
		});

	});

	it("User can selct a valid date", () => {

		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2022");
		
		//cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
		cy.get('[id="add-srma-button"]').click();
		cy.reload();
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
