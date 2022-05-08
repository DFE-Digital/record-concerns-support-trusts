describe("User can resolve an SRMA and is given validation based on options chosen", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm ="Accrington St Christopher's Church Of England High School";
	let term = "";
	let $status = "";
	let concatDate = ["date1", "date2"];
	let concatEndDate = "";
	let arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];

	it("User enters the case page", () => {
		cy.checkForExistingCase();
	});

	it("User has option to add an action item to case", () => {
		cy.reload();
		cy.get('[class="govuk-button"][role="button"]').click();
		cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
	});


	it("User clicking add to case is taken to the action page", function () {
		cy.get('button[data-prevent-double-click*="true"]').click();

		const err = '[class="govuk-list govuk-error-summary__list"]';
		cy.log((err).length);

			if ((err).length > 0 ) { 

				cy.visit(Cypress.env('url'), { timeout: 30000 });

				cy.checkForExistingCase(true);
				cy.get('[class="govuk-button"][role="button"]').click();
				cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
				cy.get('button[data-prevent-double-click*="true"]').click();
			}else{
				cy.log("No SRMA exists")
			}
	});

	it("User can set SRMA status to  Trust Considering", function () {

		cy.get('[id*="status"]').eq(0).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(0).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");
			cy.log(self.stText);
		})	
	});

	it("User can enter a valid date", function () {
		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2022");

		cy.get('[id="dtr-day"]').invoke('val').then(dtrday => {
			cy.wrap(dtrday.trim()).as("day");
		});

		cy.get('[id="dtr-month"]').invoke('val').then(dtrmon => {
			cy.wrap(dtrmon.trim()).as("month");
		});

		cy.get('[id="dtr-year"]').invoke('val').then(dtryr => {
			cy.wrap(dtryr.trim()).as("year");
		});
		
		cy.log(this.day+"-"+this.month+"-"+this.year);	
		concatDate = (this.day+"-"+this.month+"-"+this.year);
		cy.log(concatDate);
	});

	it("User can successfully add SRMA to a case", () => {
		cy.get('[id="add-srma-button"]').click();
	});

	it("User can click on a link to view a live SRMA record", function () {

		cy.get('table:nth-child(4) > tbody').children().should(($srma) => {
            expect($srma).to.be.visible
			expect($srma.text()).to.contain("SRMA");
        })

		cy.get('table:nth-child(4) > tbody > tr > td:nth-child(2)').should(($status) => {
            expect($status).to.be.visible
			expect($status.text().trim()).to.contain(this.stText);
        })

		cy.get('a[href*="/action/srma/"]').click();

	});

	it("If no entry is recorded in a section, the user should see a grey Empty note in the middle column", function () {
		cy.log("test");
	});



	it("User can set status to Trust Considering", function () {
		cy.get('[class="govuk-link"]').eq(0).click();

		cy.get('[id*="status"]').eq(0).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(0).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");

			cy.log("Status set as "+term);
			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(0).text().trim()).to.contain("Trust considering").and.to.match(/Status/i);
			});
		})

		it("User on the SRMA page cannot decline SRMA without reason populated", function () {
			cy.get('[id="complete-decline-srma-button"]').click();
	
			//Tests that there is error validation to force entry of both dates
			const err = '[class="govuk-list govuk-error-summary__list"]';   
			cy.log((err).length);
	
				if ((err).length > 0 ) { 
	
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
				}else{
					//this code path is a fallback in case the data is altered mid test
					cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
				}
	
				cy.reload();
		});
	
	});

	it("User on the SRMA page cannot decline SRMA without reason populated", function () {
		cy.get('[id="complete-decline-srma-button"]').click();

		//Tests that there is error validation to force entry of both dates
		const err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

			if ((err).length > 0 ) { 

				cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible')
				cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
			}else{
				//this code path is a fallback in case the data is altered mid test
				cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
			}

			cy.reload();
	});


	it("User can set status to preparing for deployment", () => {
		cy.get('[class="govuk-link"]').eq(0).click();

		cy.get('[id*="status"]').eq(1).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(1).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");

			cy.log("Status set as "+term);
			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(0).text().trim()).to.contain(term.trim()).and.to.match(/Status/i);
			});
		})

	});

	it("User on the SRMA page cannot decline SRMA without reason populated", function () {
		cy.get('[id="complete-decline-srma-button"]').click();

		//Tests that there is error validation to force entry of both dates
		const err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

			if ((err).length > 0 ) { 

				cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible')
				cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
			}else{
				//this code path is a fallback in case the data is altered mid test
				cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
			}

			cy.reload();
	});

	it("User on the SRMA page cannot cancel SRMA without reason populated", function () {
		cy.get('[href*="/cancel"]').click();

		//Tests that there is error validation to force entry of both dates
		const err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

			if ((err).length > 0 ) { 

				cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible')
				cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
			}else{
				//this code path is a fallback in case the data is altered mid test
				cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
			}
			cy.reload();
	});

	it("User CAN cancel with reason populated", function () {

		cy.get('[class="govuk-link"]').eq(2).click();

		let rand = Math.floor(Math.random()*2)

		cy.get('[id^="reason"]').eq(rand).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");

			cy.log("Reason set as "+term);

			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(2).text().trim()).to.contain(term.trim()).and.to.match(/Reason/i);
				expect($row.eq(0).text().trim()).to.contain("Preparing for deployment").and.to.match(/Status/i);
			});
		})

	});


	it("User CAN cancel with reason populated", function () {

		cy.get('[class="govuk-link"]').eq(2).click();


	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});
