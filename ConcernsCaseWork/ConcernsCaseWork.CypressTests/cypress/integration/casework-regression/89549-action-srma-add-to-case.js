describe("User can add action srma to existing case", () => {
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

		cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the date SRMA was offered to the trust');
		cy.reload();
	});

	it("User is shown validation and cannot add more tan 500 characters in the Notes section", () => {
		const lstring =
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx';

        cy.get('#srma-notes').should('be.visible');
        cy.get('#srma-notes-info').then(($inf) =>{
            expect($inf).to.be.visible
            expect($inf.text()).to.match(/(500 characters remaining)/i)   

            let text = Cypress._.repeat(lstring, 10)
            expect(text).to.have.length(500);
            
        cy.get('#srma-notes').invoke('val', text);
        cy.get('#srma-notes').type('{shift}{alt}'+ '1');
       
		})


	});

	it("User is given a warning for remaining text", () => {
		cy.get('#srma-notes-info').then(($inf2) =>{
			expect($inf2).to.be.visible
			expect($inf2.text()).to.match(/(1 character too many)/i);      
			})
			cy.get('[id="add-srma-button"]').click();
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Notes must be 500 characters or less');
			cy.reload();

	});


	it("User can select an SRMA status", function () {

		let rand = Math.floor(Math.random()*2)

		cy.get('[id*="status"]').eq(rand).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
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

	it("User on a live SRMA page can see a list of items", function () {

		cy.get('[class="govuk-table__row"]').should(($row) => {

			expect($row).to.have.length(7);
            expect($row.eq(0).text().trim()).to.contain(this.stText).and.to.match(/Status/i);
			expect($row.eq(1).text().trim()).to.contain('Date offered').and.to.match(/(Date offered)/i);
			expect($row.eq(2).text().trim()).to.contain('Reason').and.to.match(/(Reason)/i);
			expect($row.eq(3).text().trim()).to.contain('Date accepted').and.to.match(/(Date accepted)/i);
			expect($row.eq(4).text().trim()).to.contain('Dates of visit').and.to.match(/(Dates of visit)/i);
			expect($row.eq(5).text().trim()).to.contain('Date report sent to trust').and.to.match(/(Date report sent to trust)/i);
			expect($row.eq(6).text().trim()).to.contain('Notes').and.to.match(/(Notes)/i);
	});

});


	it("User on a live SRMA page can see conditional Edit/Add links", function () {

			cy.get('[class="govuk-table__cell"]').each(($Cell, index) => {
			cy.log("cell count is"+$Cell.length);		
					cy.get('tr.govuk-table__row').eq(index).then(($row) => {
						
						if (($Cell).find('.govuk-tag.ragtag.ragtag__grey').length > 0) {
							cy.get('tr.govuk-table__row').eq(index).should('contain.text', 'Add');	
							cy.log("Empty");
							
						}else{
							cy.get('tr.govuk-table__row').eq(index).should('contain.text', 'Edit');	
							cy.log("Not Empty");
						}
					})
			});
});

	it("If no entry is recorded in a section, the user should see a grey Empty note in the middle column", function () {
		cy.log("test");
	});



	it("User can Add/Edit SRMA Status", function () {

		cy.get('[class="govuk-link"]').eq(0).click();

		let rand = Math.floor(Math.random()*2)

		cy.get('[id*="status"]').eq(rand).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");

			cy.log("Status set as "+term);
			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(0).text().trim()).to.contain(term.trim()).and.to.match(/Status/i);
			});
		})

	});

		it("User can Add/Edit SRMA Date offered", function () {

			cy.get('[class="govuk-link"]').eq(1).click();

			cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
			cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
			cy.get('[id="dtr-year"]').type("2021");
	
			cy.get('[id="add-srma-button"]').click();// We need to retrun to the page to handle value updates
			cy.get('[class="govuk-link"]').eq(1).click();

			cy.get('[id="dtr-day"]').invoke('val').then(dtrday => {
				cy.wrap(dtrday.trim()).as("day");
				arrDate[0] = dtrday;
			});
	
			cy.get('[id="dtr-month"]').invoke('val').then(dtrmon => {
				cy.wrap(dtrmon.trim()).as("month");
				arrDate[1] = dtrmon;
			});
	
			cy.get('[id="dtr-year"]').invoke('val').then(dtryr => {
				cy.wrap(dtryr.trim()).as("year");
				arrDate[2] = dtryr;
			});
						
			concatDate = (arrDate[0]+"-"+arrDate[1]+"-"+arrDate[2]);
			cy.log(concatDate);
	
				cy.get('[id="add-srma-button"]').click();
				cy.get('[class="govuk-table__row"]').should(($row) => {
					expect($row.eq(1).text().trim()).to.contain(concatDate.trim()).and.to.match(/Date offered/i);
				});
			})

	it("User can Add/Edit SRMA Reason", function () {

		cy.get('[class="govuk-link"]').eq(2).click();

		let rand = Math.floor(Math.random()*2)

		cy.get('[id^="reason"]').eq(rand).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");

			cy.log("Reason set as "+term);
			//cy.log("inside then=> this.stText2 ="+this.stText);//used for assrtions outside of this test scope

			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(2).text().trim()).to.contain(term.trim()).and.to.match(/Reason/i);
			});
		})

	});


	it("User can Add/Edit SRMA Date accepted", function () {

		cy.get('[class="govuk-link"]').eq(3).click();

		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2021");

		cy.get('[id="add-srma-button"]').click();// We need to retrun to the page to handle value updates
		cy.get('[class="govuk-link"]').eq(3).click();

		cy.get('[id="dtr-day"]').invoke('val').then(dtrday => {
			cy.wrap(dtrday.trim()).as("day");
			arrDate[0] = dtrday;
		});

		cy.get('[id="dtr-month"]').invoke('val').then(dtrmon => {
			cy.wrap(dtrmon.trim()).as("month");
			arrDate[1] = dtrmon;
		});

		cy.get('[id="dtr-year"]').invoke('val').then(dtryr => {
			cy.wrap(dtryr.trim()).as("year");
			arrDate[2] = dtryr;
		});
		
		concatDate = (arrDate[0]+"-"+arrDate[1]+"-"+arrDate[2]);


		cy.log(concatDate);

			cy.get('[id="add-srma-button"]').click();
			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(3).text().trim()).to.contain(concatDate.trim()).and.to.match(/Date accepted/i);
			});


	});


	it("User can Add/Edit SRMA Date of visit", function () {

			cy.get('[class="govuk-link"]').eq(4).click();
	
			//Start date
			cy.get('[id="start-dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
			cy.get('[id="start-dtr-month"]').type(Math.floor(Math.random() *3) + 10);
			cy.get('[id="start-dtr-year"]').type("2021");

			cy.get('[id="add-srma-button"]').click(); // We need to retrun to the page to handle value updates
	

			//Tests that there is no error validation to force entry of both dates
			const err = '[class="govuk-list govuk-error-summary__list"]';   
			cy.log((err).length);
	
				if ((err).length > 0 ) { 
	
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.exist')
				}

			cy.get('[class="govuk-link"]').eq(4).click();


			//End date
			cy.get('[id="end-dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
			cy.get('[id="end-dtr-month"]').type(Math.floor(Math.random() *3) + 10);
			cy.get('[id="end-dtr-year"]').type("2023");

			// return to Page
			cy.get('[id="add-srma-button"]').click(); // We need to retrun to the page to handle value updates

			cy.get('[class="govuk-link"]').eq(4).click();
	
			//Validation of Start date
			cy.get('[id="start-dtr-day"]').invoke('val').then(dtrday => {
				cy.wrap(dtrday.trim()).as("day");
				arrDate[0] = dtrday;
			});
	
			cy.get('[id="start-dtr-month"]').invoke('val').then(dtrmon => {
				cy.wrap(dtrmon.trim()).as("month");
				arrDate[1] = dtrmon;
			});
	
			cy.get('[id="start-dtr-year"]').invoke('val').then(dtryr => {
				cy.wrap(dtryr.trim()).as("year");
				arrDate[2] = dtryr;
			});
						
			concatDate = (arrDate[0]+"-"+arrDate[1]+"-"+arrDate[2]);
			cy.log(concatDate);


			//Validation of End date
			cy.get('[id="end-dtr-day"]').invoke('val').then(dtrday => {
				cy.wrap(dtrday.trim()).as("day");
				arrDate[3] = dtrday;
			});
	
			cy.get('[id="end-dtr-month"]').invoke('val').then(dtrmon => {
				cy.wrap(dtrmon.trim()).as("month");
				arrDate[4] = dtrmon;
			});
	
			cy.get('[id="end-dtr-year"]').invoke('val').then(dtryr => {
				cy.wrap(dtryr.trim()).as("year");
				arrDate[5] = dtryr;
			});
						
			concatEndDate = (arrDate[3]+"-"+arrDate[4]+"-"+arrDate[5]);
			cy.log(concatEndDate);
	
			cy.get('[id="add-srma-button"]').click();

			//back on srma page validation
			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(4).text().trim()).to.contain(concatDate.trim()).and.to.match(/Dates of visit/i);
			});

			//cy.get('[id="add-srma-button"]').click();
			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(4).text().trim()).to.contain(concatEndDate.trim()); 
			});




	});



	it("User can Add/Edit SRMA Date report sent to trust", function () {

		cy.get('[class="govuk-link"]').eq(5).click();

		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2021");

		cy.get('[id="add-srma-button"]').click();// We need to retrun to the page to handle value updates
		cy.get('[class="govuk-link"]').eq(5).click();

		cy.get('[id="dtr-day"]').invoke('val').then(dtrday => {
			cy.wrap(dtrday.trim()).as("day");
			arrDate[0] = dtrday;
		});

		cy.get('[id="dtr-month"]').invoke('val').then(dtrmon => {
			cy.wrap(dtrmon.trim()).as("month");
			arrDate[1] = dtrmon;
		});

		cy.get('[id="dtr-year"]').invoke('val').then(dtryr => {
			cy.wrap(dtryr.trim()).as("year");
			arrDate[2] = dtryr;
		});
					
		concatDate = (arrDate[0]+"-"+arrDate[1]+"-"+arrDate[2]);
		cy.log(concatDate);

			cy.get('[id="add-srma-button"]').click();
			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(5).text().trim()).to.contain(concatDate.trim()).and.to.match(/Date report sent to trust/i);
			});

	});

	/*
*/
	it("User can Add/Edit SRMA Notes", function () {

		let date = new Date();
		const lstring ='Cypress test run '+date;

		cy.get('[class="govuk-link"]').eq(6).click();
        cy.get('#srma-notes').should('be.visible');

        cy.get('#srma-notes-info').then(($inf) =>{         
        cy.get('#srma-notes').invoke('val', lstring);
       
		})


		cy.get('#srma-notes').invoke('val').then(value => {
			//cy.wrap(value.trim()).as("term");
			term = value;
		});

		cy.get('[id="add-srma-button"]').click();
				cy.get('[class="govuk-table__row"]').should(($row) => {
					expect($row.eq(6).text().trim()).to.contain(term.trim()).and.to.match(/Notes/i);
				});
	});

	it("User on the SRMA page can see conditional buttons based on Status", function () {
		cy.log("test");
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});
