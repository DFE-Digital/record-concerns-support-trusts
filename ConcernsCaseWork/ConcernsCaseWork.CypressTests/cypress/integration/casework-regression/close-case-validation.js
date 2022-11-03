describe("Users can see warning messages on the case closure page", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const lstring =
		'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx';

	//Opens the first active case in the list
	it("Opens an active case", () => {
		cy.get('.govuk-link[href^="case"]').eq(0).click();
	});

	it("User can close any open concerns", () => {
		cy.closeAllOpenConcerns();
		});



	it("User can enter case closure page", () => {
		//cy.closeSRMA();

		const $elem = Cypress.$('table:nth-child(4) > thead > tr > th:nth-child(1)');

   		cy.log(($elem).length)
			if (Cypress.$($elem).length > 1 ) { //Cypress.$ needed to handle element missing exception (>1 due to DOM)

			cy.log('Found Open Actions');

						if (Cypress.$('[href*="/closed"]').length > 0 ) {

							cy.log('Did not find active SRMA')
						}else {
							cy.log('SRMA is present')
							cy.closeSRMA();
						}

			}else {
			cy.log('No Open Actions Table Present');

			}

		cy.get('#close-case-button').click()
		
		//cy.get('#close-case-button').click()
	})

	it("User can type 200 characters max into the outcome box", function () {
		cy.get("#case-outcomes-info").then(($info) =>{
            expect($info).to.be.visible
            expect($info.text()).to.match(/(200 characters remaining)/i)   

            let text = Cypress._.repeat(lstring, 4)
            expect(text).to.have.length(200);
            
        cy.get('#case-outcomes').invoke('val', text);
        cy.get('#case-outcomes').type('{shift}{alt}'+ '1');
		})
	});

	it("User can see the character count reduce accordingly", () => {
        cy.get("#case-outcomes-info").then(($info2) =>{
            expect($info2).to.be.visible
            expect($info2.text()).to.match(/(1 character too many)/i);      
            })
        })

	it("User can see the text box expand when typing large numbers", () => {
		cy.get('#case-outcomes').should(($box) => {
			expect($box).to.not.have.attr('style', 'height: 113px; border-color: black;')
			})
			cy.get('#case-outcomes').should('have.attr', 'style').and('match', /(138px|139px|140px|141px|142px)/i)
			cy.get('#case-outcomes').should('have.attr', 'style').and('match', /(border-color: green)/i)
		})

	it("User closing a case with outcome exceeding max characters is shown error", () => {
	    cy.get('#close-case-button').click();
	    cy.get('#errorSummary').children().then(($error) =>{
		   expect($error).to.be.visible
		   expect($error.text()).to.match(/(too many characters)/i);
		   cy.get('#case-outcomes').scrollIntoView().clear().wait(1000);
		})
	})

	it("User closing a case with no outcome text is shown an error", () => {
		cy.reload()
	    cy.get('#close-case-button').click();
	    cy.get('#errorSummary').children().then(($error) =>{
		   expect($error).to.be.visible
		   expect($error.text()).to.match(/(not recorded rationale for closure)/i);
		})

	})
})

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

