import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import srmaAddPage from "/cypress/pages/caseActions/srmaAddPage";
import srmaEditPage from "/cypress/pages/caseActions/srmaEditPage";

describe("User can resolve an SRMA when Status is Deployed and resolution reason is Complete", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let concatDate = "";
	let arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];
	let returnedDate = ["date1", "date2"];
	let returnedDateEnd


	it("User enters the case page", () => {
		cy.checkForExistingCase();
	});

	it("User has option to add an action item to case", () => {
		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('Srma')
		AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);

	});

	it("User clicking add to case is taken to the action page", function () {

				AddToCasePage.getAddToCaseBtn().click();
				cy.log(utils.checkForGovErrorSummaryList() );

				if (utils.checkForGovErrorSummaryList() > 0 ) { 
					cy.log("Case Action already exists");

							cy.visit(Cypress.env('url'), { timeout: 30000 });
							cy.checkForExistingCase(true);
							CaseManagementPage.getAddToCaseBtn().click();
							AddToCasePage.addToCase('Srma');
							AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);
							AddToCasePage.getAddToCaseBtn().click();

				}else {
					cy.log("No Case Action exists");	
					cy.log(utils.checkForGovErrorSummaryList() );
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

		CaseManagementPage.getOpenActionsTable().children().should(($srma) => {
			expect($srma).to.be.visible
			expect($srma.text()).to.contain("SRMA");
		})

		CaseManagementPage.getOpenActionsTable().should(($status) => {
			expect($status).to.be.visible
			expect($status.text().trim()).to.contain(this.stText);
		})
		CaseManagementPage.getOpenActionLink("srma").click();
	});


	it("User can set status to DEPLOYED", function () {
		cy.get('[class="govuk-link"]').eq(0).click();

		cy.get('[id*="status"]').eq(2).click();

		cy.get('label.govuk-label.govuk-radios__label').eq(2).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");

			cy.log("Status set as "+term);
			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(0).text().trim()).to.contain("Deployed").and.to.match(/Status/i);
			});
		});
	});


		it("User on the SRMA page cannot COMPLETE without mandatory items set", function () {
			cy.get('[id="complete-decline-srma-button"]').click();
	
			//Tests that there is error validation to force entry of both dates
			const err = '[class="govuk-list govuk-error-summary__list"]';   
			cy.log((err).length);
	
				if ((err).length > 0 ) { 
	
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date accepted');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date of visit');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
				}else{
					//this code path is a fallback in case the data is altered mid test
					cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible');
				}
	
				cy.reload();
		});
	
		it("User can Add SRMA Reason to remove validation", function () {

			cy.get('[class="govuk-link"]').eq(2).click();
	
			let rand = Math.floor(Math.random()*2);
	
			cy.get('[id^="reason"]').eq(rand).click();
			cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
				cy.wrap(term.trim()).as("stText");
				cy.log("Reason set as "+term);
				cy.get('[id="add-srma-button"]').click();

				cy.get('[class="govuk-table__row"]').should(($row) => {
					expect($row.eq(2).text().trim()).to.contain(term.trim()).and.to.match(/Reason/i);
				});
			});

			cy.get('[id="complete-decline-srma-button"]').click();
	
			//Tests that there is error validation to force entry of both dates
			const err = '[class="govuk-list govuk-error-summary__list"]';   
			cy.log((err).length);
	
				if ((err).length > 0 ) { 
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter the reason');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date accepted');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date of visit');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
				}else{
					//this code path is a fallback in case the data is altered mid test
					cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible');
				}
	
				cy.reload();
		});


	it("User can Add SRMA Date accepted to remove validation", function () {

			srmaEditPage.getTableAddEditLink().eq(3).click();
			cy.log("date offered closure ").then(() => {
				cy.log(srmaAddPage.setDateAccepted() ).then((returnedVal) => { 
					cy.log("logging date offered closure inside nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{

						returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
					});
				});
			});
			srmaAddPage.getAddCaseActionBtn().click();
			srmaEditPage.getSrmaTableRow().should(($row) => {
				expect($row.eq(3).text().trim()).to.contain(returnedDate.trim()).and.to.match(/Date accepted/i);
				})

			cy.log("logging date offered out 0 +self.stText "+self.stText);
			cy.log("logging date offered out 0returnedDate "+returnedDate);
			});

			//Tests that validation is removed //  << Needs abstracting out into commands/functions 	
			cy.get('[id="complete-decline-srma-button"]').click();
	
			//Tests that there is error validation to force entry of both dates
			const err = '[class="govuk-list govuk-error-summary__list"]';   
			cy.log((err).length);
	
				if ((err).length > 0 ) { 
	
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter the reason');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter date accepted');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date of visit');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
				}else{
					//this code path is a fallback in case the data is altered mid test
					cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
				}
				cy.reload();
		});

		it( "User can Add SRMA Date of visit to remove validation", function () {

			srmaEditPage.getTableAddEditLink().eq(4).click();

			cy.log("dates of visit Start Date closure ").then(() => {
				cy.log(srmaAddPage.setDateVisitStart() ).then((returnedVal) => { 
					cy.log("logging nested "+returnedVal).then(() =>{
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{

						returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal "+returnedVal)
						cy.log("set date logging date offered out 0 returnedDate "+returnedDate);
						});
					});
				});
			});

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

			//back on srma page validation
				srmaEditPage.getSrmaTableRow().should(($row) => {
				expect($row.eq(4).text().trim()).to.contain(returnedDate.trim()).and.to.match(/Dates of visit/i);
				});
	

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(4).text().trim()).to.contain(returnedDateEnd.trim()); 
				});
			});

			cy.get('[id="complete-decline-srma-button"]').click();

			let err = '[class="govuk-list govuk-error-summary__list"]';   
			cy.log((err).length);
	
				if ((err).length > 0 ) { 
	
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter the reason');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter date accepted');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter date of visit');
					cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
				}else{
					//this code path is a fallback in case the data is altered mid test
					cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(4).should('not.be.visible');
				}
				cy.reload();
		});

		it( "User can Add SRMA Date report sent to trust to remove validation", function () {

			cy.get('[id="complete-decline-srma-button"]').click();
			cy.get('[class="govuk-link"]').eq(5).click();
			cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
			cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
			cy.get('[id="dtr-year"]').type("2021");
			cy.get('[id="add-srma-button"]').click();// We need to retrun to the page to handle value updates
			cy.get('[class="govuk-link"]').eq(5).click();
			cy.get('[id="dtr-day"]').invoke('val').then(dtrday => {
				cy.wrap(dtrday.trim()).as("day");
				arrDate[0] = dtrday;

				cy.log("debug for arrDate: "+arrDate[0]);
			});
	
			cy.get('[id="dtr-month"]').invoke('val').then(dtrmon => {
				cy.wrap(dtrmon.trim()).as("month");
				arrDate[1] = dtrmon;
			});
	
			cy.get('[id="dtr-year"]').invoke('val').then(dtryr => {
				cy.wrap(dtryr.trim()).as("year");
				arrDate[2] = dtryr;

		concatDate = arrDate[0]+"-"+arrDate[1]+"-"+arrDate[2];
		cy.log("debug for date trust : "+concatDate);
			});

				cy.get('[id="add-srma-button"]').click();

				cy.get('[class="govuk-table__row"]').should(($row) => {
					expect($row.eq(5).text().trim()).to.contain(concatDate.trim()).and.to.match(/Date report sent to trust/i);
				});

				const err = '[class="govuk-list govuk-error-summary__list"]';   
				cy.log((err).length);
		
					if ((err).length > 0 ) { 
		
						cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.exist');

					}else{
						//this code path is a fallback in case the data is altered mid test
						cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(5).should('not.be.visible');
					}
			});

		it("User is navigated to resolve page when status is Deployed, Closure reason Complete", () => {

			cy.get('[id="complete-decline-srma-button"]').click();
			cy.get('[class="govuk-label govuk-checkboxes__label"]').should('contain.text', 'Confirm SRMA is complete');
			cy.get('[id="add-srma-button"]').click();

			const err = '[class="govuk-list govuk-error-summary__list"]';   
					cy.log((err).length);
			
						if ((err).length > 0 ) { 
			
							cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
							cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Confirm SRMA is complete');

						}else{
							//this code path is a fallback in case the data is altered mid test
							cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(5).should('not.be.visible');
						}	
			cy.reload(); //needed to handle validation bug
			cy.get('[id="confirmChk"]').click();
			cy.get('[id="add-srma-button"]').click();
	});


		it("User is navigated back to the case page after completion,", () => {

			cy.get('[class="govuk-caption-m"]').should('be.visible');
			cy.get('[class="govuk-caption-m"]').should('contain.text', 'Case ID');
		});
			

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});