import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import srmaAddPage from "/cypress/pages/caseActions/srmaAddPage";
import srmaEditPage from "/cypress/pages/caseActions/srmaEditPage";
import { LogTask } from "../../../support/constants";

describe("User can resolve an SRMA when Status is Deployed and resolution reason is Complete", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let returnedDate = ["date1", "date2"];
	let returnedDateEnd


	it("Should validate SRMA Deployed complete", () => {
        cy.task(LogTask, "User creates a case");
        cy.visit(Cypress.env('url'), { timeout: 30000 });
        cy.checkForExistingCase(true);
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Srma');
        AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);
        AddToCasePage.getAddToCaseBtn().click();

		cy.task(LogTask, "User clicking add to case is taken to the action page");

		cy.task(LogTask, "User can set SRMA status to  Trust Considering");
		cy.get('[id*="status"]').eq(0).click();

		cy.task(LogTask, "User can enter a valid date");
		cy.get('[id="dtr-day"]').clear().type("10");
		cy.get('[id="dtr-month"]').clear().type("12");
		cy.get('[id="dtr-year"]').clear().type("2022");

		cy.task(LogTask, "User can successfully add SRMA to a case");

		cy.get('[id="add-srma-button"]').click();
		cy.task(LogTask, "User can click on a link to view a live SRMA record");



		CaseManagementPage.getOpenActionsTable().children().should(($srma) => {
			expect($srma).to.be.visible
			expect($srma.text()).to.contain("SRMA");
		})

		CaseManagementPage.getOpenActionsTable().should(($status) => {
			expect($status).to.be.visible
			expect($status.text().trim()).to.contain("Trust considering");
		})
		CaseManagementPage.getOpenActionLink("srma").click();

		cy.task(LogTask, "User can set status to DEPLOYED");

		cy.get('[class="govuk-link"]').eq(0).click();

		cy.get('[id*="status"]').eq(2).click();

		cy.get('label.govuk-label.govuk-radios__label').eq(2).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");

			cy.log("Status set as " + term);
			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(0).text().trim()).to.contain("Deployed").and.to.match(/Status/i);
			});
		});

		cy.task(LogTask, "User on the SRMA page cannot COMPLETE without mandatory items set");


		cy.get('[id="complete-decline-srma-button"]').click();

		//Tests that there is error validation to force entry of both dates
		let err = '[class="govuk-list govuk-error-summary__list"]';
		cy.log((err).length);

		if ((err).length > 0) {

			cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the reason');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date accepted');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date of visit');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
		} else {
			//this code path is a fallback in case the data is altered mid test
			cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible');
		}

		cy.reload();
		cy.task(LogTask, "User can Add SRMA Reason to remove validation");



		cy.get('[class="govuk-link"]').eq(2).click();

		let rand = Math.floor(Math.random() * 2);

		cy.get('[id^="reason"]').eq(rand).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
			cy.wrap(term.trim()).as("stText");
			cy.log("Reason set as " + term);
			cy.get('[id="add-srma-button"]').click();

			cy.get('[class="govuk-table__row"]').should(($row) => {
				expect($row.eq(2).text().trim()).to.contain(term.trim()).and.to.match(/Reason/i);
			});
		});

		cy.get('[id="complete-decline-srma-button"]').click();

		//Tests that there is error validation to force entry of both dates
		err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

		if ((err).length > 0) {
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter the reason');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date accepted');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date of visit');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
		} else {
			//this code path is a fallback in case the data is altered mid test
			cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible');
		}

		cy.reload();


		cy.task(LogTask, "User can Add SRMA Date accepted to remove validation");


		srmaEditPage.getTableAddEditLink().eq(3).click();
		cy.log("date offered closure ").then(() => {
			cy.log(srmaAddPage.setDateAccepted()).then((returnedVal) => {
				cy.log("logging date offered closure inside nested " + returnedVal).then(() => {
					cy.wrap(returnedVal.trim()).as("returnedDate").then(() => {

						returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal " + returnedVal)
						cy.log("set date logging date offered out 0 returnedDate " + returnedDate);
					});
				});
			});
			srmaAddPage.getAddCaseActionBtn().click();
			srmaEditPage.getSrmaTableRow().should(($row) => {
				expect($row.eq(3).text().trim()).to.contain(returnedDate.trim()).and.to.match(/Date accepted/i);
			})

			cy.log("logging date offered out 0 +self.stText " + self.stText);
			cy.log("logging date offered out 0returnedDate " + returnedDate);
		});

		//Tests that validation is removed //  << Needs abstracting out into commands/functions 	
		cy.get('[id="complete-decline-srma-button"]').click();

		//Tests that there is error validation to force entry of both dates
		//const err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

		if ((err).length > 0) {

			cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter the reason');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter date accepted');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date of visit');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
		} else {
			//this code path is a fallback in case the data is altered mid test
			cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible')
		}
		cy.reload();

		cy.task(LogTask, "User can Add SRMA Date of visit to remove validation");


		srmaEditPage.getTableAddEditLink().eq(4).click();

		cy.log("dates of visit Start Date closure ").then(() => {
			cy.log(srmaAddPage.setDateVisitStart()).then((returnedVal) => {
				cy.log("logging nested " + returnedVal).then(() => {
					cy.wrap(returnedVal.trim()).as("returnedDate").then(() => {

						returnedDate = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal " + returnedVal)
						cy.log("set date logging date offered out 0 returnedDate " + returnedDate);
					});
				});
			});
		});

		cy.log("dates of visit End Date closure ").then(() => {
			cy.log(srmaAddPage.setDateVisitEnd()).then((returnedVal) => {
				cy.log("logging nested " + returnedVal).then(() => {
					cy.wrap(returnedVal.trim()).as("returnedDate").then(() => {

						returnedDateEnd = returnedVal;
						cy.log("set Date logging date offered out 0 returnedVal " + returnedVal)
						cy.log("set date logging date offered out 0 returnedDate " + returnedDate);
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

		err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

		if ((err).length > 0) {

			cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter the reason');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter date accepted');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.contain.text', 'Enter date of visit');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter date report sent to trust');
		} else {
			//this code path is a fallback in case the data is altered mid test
			cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(4).should('not.be.visible');
		}
		cy.reload();
		cy.task(LogTask, "User can Add SRMA Date report sent to trust to remove validation");


		cy.get('[id="complete-decline-srma-button"]').click();
		cy.get('[class="govuk-link"]').eq(5).click();

		cy.get('[id="dtr-day"]').clear().type("09");
		cy.get('[id="dtr-month"]').clear().type("02");
		cy.get('[id="dtr-year"]').clear().type("2021");

		cy.get('[id="add-srma-button"]').click();// We need to retrun to the page to handle value updates
		cy.get('[class="govuk-link"]').eq(5).click();

		cy.get('[id="add-srma-button"]').click();

		cy.get('[class="govuk-table__row"]').should(($row) => {
			expect($row.eq(5).text().trim()).to.contain("09-02-2021").and.to.match(/Date report sent to trust/i);
		});

		err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

		if ((err).length > 0) {

			cy.get('[class="govuk-list govuk-error-summary__list"]').should('not.exist');

		} else {
			//this code path is a fallback in case the data is altered mid test
			cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(5).should('not.be.visible');
		}

		cy.task(LogTask, "User is navigated to resolve page when status is Deployed, Closure reason Complete");


		cy.get('[id="complete-decline-srma-button"]').click();
		cy.get('[class="govuk-label govuk-checkboxes__label"]').should('contain.text', 'Confirm SRMA is complete');
		cy.get('[id="add-srma-button"]').click();

		err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

		if ((err).length > 0) {

			cy.get('[class="govuk-list govuk-error-summary__list"]').should('be.visible');
			cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Confirm SRMA is complete');

		} else {
			//this code path is a fallback in case the data is altered mid test
			cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(5).should('not.be.visible');
		}
		cy.reload(); //needed to handle validation bug
		cy.get('[id="confirmChk"]').click();
		cy.get('[id="add-srma-button"]').click();

		cy.task(LogTask, "User is navigated back to the case page after completion");



		cy.get('[class="govuk-caption-m"]').should('be.visible');
		cy.get('[class="govuk-caption-m"]').should('contain.text', 'Case ID');

	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});