import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
<<<<<<< Updated upstream
=======
import utils from "/cypress/support/utils"
import srmaAddPage from "/cypress/pages/caseActions/srmaAddPage";
import srmaEditPage from "/cypress/pages/caseActions/srmaEditPage";
>>>>>>> Stashed changes

describe("User can add case actions to an existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let term = "";
<<<<<<< Updated upstream
	let $status = "";
	let concatDate = ["date1", "date2"];
	let concatEndDate = "";
	let arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];
=======
	let returnedDate = ["date1", "date2"];
	let returnedDateEnd
	let stText = "null";
>>>>>>> Stashed changes

	it("User enters the case page", () => {
		cy.checkForExistingCase();
	});

	it("User has option to add SRMA Case Action to a case", () => {
		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		//cy.get('[class="govuk-button"][role="button"]').click();

		AddToCasePage.addToCase('Srma')
		AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);
	});

<<<<<<< Updated upstream

	it("User is taken to the correct Case Action page", function () {
		cy.get('button[data-prevent-double-click*="true"]').click();

		cy.wait(500);
			const err = '.govuk-list.govuk-error-summary__list';
			cy.log(err.length);
=======
	it("User can click to add Financial Plan Case Action to a case", function () {
		
		AddToCasePage.getAddToCaseBtn().click();
		cy.log(utils.checkForGovErrorSummaryList() );
>>>>>>> Stashed changes

			if (err.length > 0) { //Cypress.$ needed to handle element missing exception
				
				cy.log("SRMA exists")

					cy.visit(Cypress.env('url'), { timeout: 30000 });
					cy.checkForExistingCase(true);
<<<<<<< Updated upstream
					cy.get('[class="govuk-button"][role="button"]').click();

					AddToCasePage.getHeadingText().should('contain.text', 'Add to case');
				}else{
					cy.log("No SRMA exists")	
					AddToCasePage.getHeadingText().should('contain.text', 'Add to case');
				}
			});

/*

	it("User is shown validation and cannot proceed without selecting a status", () => {
		cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
		cy.get('button[data-prevent-double-click*="true"]').click();

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

=======
					CaseManagementPage.getAddToCaseBtn().click();
					AddToCasePage.addToCase('Srma');
					AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);
					AddToCasePage.getAddToCaseBtn().click();

		}else {
			cy.log("No Case Action exists");	
			cy.log(utils.checkForGovErrorSummaryList() );
		}
	});

	it("User is taken to the correct Case Action page", function () {

		srmaAddPage.getHeadingText().then(term => {
			expect(term.text().trim()).to.match(/School Resource Management Adviser/i);
		});
	});

	it("User is shown validation and cannot proceed without selecting a status", () => {
		srmaAddPage.setDateOffered();
		srmaAddPage.getAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('contain.text', 'Select status');
		cy.reload(true);
	});

	it("User is shown validation and cannot proceed without selecting a valid date", () => {

		srmaAddPage.setStatusSelect("random");
		srmaAddPage.getDateOfferedDay().type("0");
		srmaAddPage.getDateOfferedMonth().type("0");
		srmaAddPage.getDateOfferedYear().type("0");
		srmaAddPage.getAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('contain.text', 'Enter the date SRMA was offered to the trust');
		cy.reload(true);
	});

>>>>>>> Stashed changes
	it("User is shown validation and cannot add more tan 500 characters in the Notes section", () => {
		const lstring =
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx';

        cy.get('#srma-notes').should('be.visible');
        cy.get('#srma-notes-info').then(($inf) =>{
            expect($inf).to.be.visible
            expect($inf.text()).to.match(/(500 characters remaining)/i)   

            let text = Cypress._.repeat(lstring, 10)
            expect(text).to.have.length(500);
<<<<<<< Updated upstream
            
        cy.get('#srma-notes').invoke('val', text);
        cy.get('#srma-notes').type('{shift}{alt}'+ '1');     
=======
			srmaAddPage.getNotesBox().invoke('val', text);   
			srmaAddPage.getNotesBox().type('{shift}{alt}'+ '1');   
>>>>>>> Stashed changes
		})
	});

	it("User is given a warning for remaining text", () => {
<<<<<<< Updated upstream
		cy.get('#srma-notes-info').then(($inf2) =>{
			expect($inf2).to.be.visible
			expect($inf2.text()).to.match(/(1 character too many)/i);      
			})
			cy.get('[id="add-srma-button"]').click();
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Notes must be 500 characters or less');
			cy.reload();
=======
		srmaAddPage.setStatusSelect("random");
		srmaAddPage.setDateOffered();
		srmaAddPage.getNotesInfo().then(($inf) =>{
			expect($inf).to.be.visible
			expect($inf.text()).to.match(/(1 character too many)/i);      
		});
			srmaAddPage.getAddCaseActionBtn().click();
			utils.getGovErrorSummaryList().should('contain.text', 'Notes must be 500 characters or less');
>>>>>>> Stashed changes
	});

	it("User can select an SRMA status", function () {

<<<<<<< Updated upstream
		let rand = Math.floor(Math.random()*2)

		cy.get('[id*="status"]').eq(rand).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");
			cy.log(self.stText);
		})	
=======
		cy.log("setStatusSelect ").then(() => {
			cy.log(srmaAddPage.setStatusSelect("random") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;

					cy.log("logging the result "+stText)
	
				});
				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging the result out 0 "+returnedVal)
				cy.log("logging the result out 0"+self.stText)
				cy.log("logging the result out 0 "+stText)

				});
				cy.log("logging the result out 1 "+self.stText)
				cy.log("logging the result out 1 "+self.stText)
				cy.log("logging the result out 1 "+stText)

			});
			cy.log("logging the result out 2 "+self.stText)
			cy.log("logging the result out 2 "+stText)
>>>>>>> Stashed changes
	});

	it("User can enter a valid date", function () {
		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2022");

<<<<<<< Updated upstream
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
=======

	it("User can enter a valid offered date", function () {
		srmaAddPage.setDateOffered();
	});


	it("User can successfully add valid SRMA actions to a case", () => {

		cy.reload(true);

		//SET STATUS
		cy.log("setStatusSelect ").then(() => {
			cy.log(srmaAddPage.setStatusSelect("random") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log(self.stText);

					cy.log(self.stText);
					cy.log("logging the result "+returnedVal)
					cy.log("logging the result "+self.stText)
					cy.log("logging the result "+stText)
				});

				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging the result out 0 "+returnedVal);
				cy.log("logging the result out 0"+self.stText);
				cy.log("logging the result out 0 "+stText);

				});
				cy.log("logging the result out 1 "+self.stText);
				cy.log("logging the result out 1 "+self.stText);
				cy.log("logging the result out 1 "+stText);
			});
			cy.log("logging the result out 2 "+self.stText);
			cy.log("logging the result out 2 "+stText);

			//SET DATE
			cy.log("date offered closure ").then(() => {
				cy.log(srmaAddPage.setDateOffered() ).then((returnedVal) => { 
					cy.log("logging date offered closure inside nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{

						returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
						});
					});
				});
			});
			cy.log("logging date offered out 0 +self.stText "+self.stText);
			cy.log("logging date offered out 0returnedDate "+returnedDate[0]);

			srmaAddPage.getAddCaseActionBtn().click();
>>>>>>> Stashed changes
	});

	it("User can successfully add SRMA to a case", () => {
		cy.get('[id="add-srma-button"]').click();
	});

	it("User can click on a link to view a live SRMA record", function () {

		cy.get('table:nth-child(4) > tbody').children().should(($srma) => {
            expect($srma).to.be.visible
			expect($srma.text()).to.contain("SRMA");
        })

			cy.get('table:nth-child(4) > tbody > tr').should(($status) => {
			expect($status).to.be.visible
			expect($status.text().trim()).to.contain(this.stText);
        })
<<<<<<< Updated upstream

		cy.get('a[href*="/action/srma/"]').click();
=======
		CaseManagementPage.getOpenActionLink("srma").click();
	});
>>>>>>> Stashed changes

	});

	it("User on a live SRMA page can see a list of items", function () {

<<<<<<< Updated upstream
		cy.get('[class="govuk-table__row"]').should(($row) => {
=======
		srmaEditPage.getSrmaTableRow().should(($row) => {
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

			cy.get('[class="govuk-table__cell"]').each(($Cell, index) => {
			cy.log("cell count is"+$Cell.length);		
					cy.get('tr.govuk-table__row').eq(index).then(($row) => {
						
						if (($Cell).find('.govuk-tag.ragtag.ragtag__grey').length > 0) {
							cy.get('tr.govuk-table__row').eq(index).should('contain.text', 'Add');	
=======
				srmaEditPage.getSrmaTableRow().each(($Trow, index) => {
					const t = $Trow.text();

				cy.log("getSrmaTableRow index "+index)
				cy.log("$Trow.text "+$Trow.text() )
				cy.log("t "+t)

						if (t.includes("Empty")) {
							srmaEditPage.getSrmaTableRow().eq(index).should('contain.text', 'Add');	
>>>>>>> Stashed changes
							cy.log("Empty");
						}else{
							cy.get('tr.govuk-table__row').eq(index).should('contain.text', 'Edit');	
							cy.log("Not Empty");
						}
<<<<<<< Updated upstream
					})
			});
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
=======
			});
	});


	it("User can Add/Edit SRMA Status", function () {
		srmaEditPage.getTableAddEditLink().eq(0).click();

		cy.log("setStatusSelect ").then(() => {
			cy.log(srmaAddPage.setStatusSelect("random") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log(self.stText);

					cy.log(self.stText);
					cy.log("logging the result "+returnedVal)
					cy.log("logging the result "+self.stText)
					cy.log("logging the result "+stText)

					srmaAddPage.getAddCaseActionBtn().click();
					srmaEditPage.getSrmaTableRow().should(($row) => {
						expect($row.eq(0).text().trim()).to.contain(stText.trim()).and.to.match(/Status/i);
						});
	
				});
				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging the result out 0 "+returnedVal)
				cy.log("logging the result out 0"+self.stText)
				cy.log("logging the result out 0 "+stText)

				});
				cy.log("logging the result out 1 "+self.stText)
				cy.log("logging the result out 1 "+self.stText)
				cy.log("logging the result out 1 "+stText)
>>>>>>> Stashed changes
			});
		})

	});

		it("User can Add/Edit SRMA Date offered", function () {
<<<<<<< Updated upstream
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
=======

			srmaEditPage.getTableAddEditLink().eq(1).click();
			cy.log("date offered closure ").then(() => {
				cy.log(srmaAddPage.setDateOffered() ).then((returnedVal) => { 
					cy.log("logging date offered closure inside nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{

						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
					});
				});
			});
		});
			cy.log("logging date offered out 0 +self.stText "+self.stText);
			cy.log("logging date offered out 0returnedDate "+returnedDate[0]);

			cy.log("date offered closure ").then(() => {
				cy.log(srmaAddPage.getDateOffered() ).then((returnedVal) => { 
					cy.log("logging date offered closure inside nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{

						returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
					});
				});
>>>>>>> Stashed changes
			});
	
			cy.get('[id="dtr-year"]').invoke('val').then(dtryr => {
				cy.wrap(dtryr.trim()).as("year");
				arrDate[2] = dtryr;
			});
<<<<<<< Updated upstream
						
			concatDate = (arrDate[0]+"-"+arrDate[1]+"-"+arrDate[2]);
			cy.log(concatDate);
	
				cy.get('[id="add-srma-button"]').click();
				cy.get('[class="govuk-table__row"]').should(($row) => {
					expect($row.eq(1).text().trim()).to.contain(concatDate.trim()).and.to.match(/Date offered/i);
				});
			})
=======
				srmaAddPage.getAddCaseActionBtn().click();
				srmaEditPage.getSrmaTableRow().should(($row) => {
					expect($row.eq(1).text().trim()).to.contain(returnedDate.trim()).and.to.match(/Date offered/i);
					})
			});
>>>>>>> Stashed changes

	it("User can Add/Edit SRMA Reason", function () {

		cy.get('[class="govuk-link"]').eq(2).click();

		let rand = Math.floor(Math.random()*2)

		cy.get('[id^="reason"]').eq(rand).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");
			cy.log("Reason set as "+term);
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

<<<<<<< Updated upstream
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
=======
	it("User is not shown validation for single date of visit entry", function () {

			//cy.get('[class="govuk-link"]').eq(4).click();
			srmaEditPage.getTableAddEditLink().eq(4).click();

			cy.log("dates of visit start closure ").then(() => {
				cy.log(srmaAddPage.setDateVisitStart() ).then((returnedVal) => { 
					cy.log("logging nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{
	
						//returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
					});
				});
			});
			srmaAddPage.getAddCaseActionBtn().click();

				cy.log("checkForGovErrorSummaryList "+utils.checkForGovErrorSummaryList() );
				if (utils.checkForGovErrorSummaryList() > 0 ) { 
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.exist')
					}
				});
			});


		it("User can Add/Edit SRMA Date of visit", function () {

			srmaEditPage.getTableAddEditLink().eq(4).click();

			cy.log("dates of visit Start Date closure ").then(() => {
				cy.log(srmaAddPage.setDateVisitStart() ).then((returnedVal) => { 
					cy.log("logging nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{
>>>>>>> Stashed changes
	
						returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
						});
					});
				});
			});

<<<<<<< Updated upstream
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
=======
>>>>>>> Stashed changes

			cy.log("dates of visit End Date closure ").then(() => {
				cy.log(srmaAddPage.setDateVisitEnd() ).then((returnedVal) => { 
					cy.log("logging nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{
	
						returnedDateEnd = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
						});
					});
				});
			srmaAddPage.getAddCaseActionBtn().click();

			srmaEditPage.getSrmaTableRow().should(($row) => {
				expect($row.eq(4).text().trim()).to.contain(returnedDate.trim())//.and.to.match(/Dates of visit/i);
				expect($row.eq(4).text().trim()).to.contain(returnedDateEnd.trim()).and.to.match(/Dates of visit/i);
				});
			});
<<<<<<< Updated upstream
	});

	it("User can Add/Edit SRMA Date report sent to trust", function () {
=======

		});

>>>>>>> Stashed changes


	it("User can Add/Edit SRMA Date report sent to trust", function () {

		srmaEditPage.getTableAddEditLink().eq(5).click();

		cy.log("date report sent closure ").then(() => {
			cy.log(srmaAddPage.setDateReportSent() ).then((returnedVal) => { 
				cy.log("logging date report sent closure inside nested "+returnedVal).then(() =>{
					cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{

					returnedDate = returnedVal;
					cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
					cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
				});
			});
		});
		srmaAddPage.getAddCaseActionBtn().click();
		srmaEditPage.getSrmaTableRow().should(($row) => {
			expect($row.eq(5).text().trim()).to.contain(returnedDate.trim()).and.to.match(/Date report sent to trust/i);
			})
	});
		cy.log("logging date offered out 0 +self.stText "+self.stText);
		cy.log("logging date offered out 0returnedDate "+returnedDate[0]);
	});


	it("User can Add/Edit SRMA Notes", function () {

		srmaEditPage.getTableAddEditLink().eq(6).click();

		cy.log("setNotes ").then(() => {
			cy.log(srmaAddPage.setNotes() ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
				});
<<<<<<< Updated upstream
	});
*/
=======
			});
		});
		srmaAddPage.getAddCaseActionBtn().click();
		srmaEditPage.getSrmaTableRow().should(($row) => {
			expect($row.eq(6).text().trim()).to.contain(term.trim()).and.to.match(/Notes/i);
		});
	});
	

>>>>>>> Stashed changes
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});
