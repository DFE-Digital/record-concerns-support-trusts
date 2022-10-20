import teamCaseworkPage from "/cypress/pages/teamCaseworkPage";
import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import ntiAddPage from "/cypress/pages/caseActions/ntiAddPage";
import CaseActionsBasePage from "/cypress/pages/caseActions/caseActionsBasePage";
import homePage from "/cypress/pages/homePage";

describe("User interactions via Create Case route", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let stText = "null";
	let condText = "null";
	let reasText = "null";
	let returnedDate = "null";
	let notesText = "null";
	let count = 0

	const searchTerm =
		"Accrington St Christopher's Church Of England High School";
		let term = ""

	it("User clicks Team Casework tab and is taken to the team casework page", () => {
		homePage.getTeamCaseworkBtn().click();
		
		homePage.getTeamCaseworkHeading().should('contain.text', 'Your team casework');

	});

	it("User clicks select colleagues is taken to the Select Colleagues page", (done) => {
		teamCaseworkPage.getSelectColleagesBtn().click();
		
		//homePage.getHeadingText().should('contain.text', 'Your team casework');
		homePage.getHeadingText().should('contain.text', 'Select Colleagues to Show in Team Casework');

			// this event will automatically be unbound when this
			// test ends because it's attached to 'cy'
			cy.on('uncaught:exception', (err, runnable) => {
			  expect(err.message).to.include('application code')
		  
			  // using mocha's async done callback to finish
			  // this test so we prove that an uncaught exception
			  // was thrown
			  done()
		  
			  // return false to prevent the error from
			  // failing this test
			  return false
			})


	});

	it("User can see a list of selectable colleagues", () => {

		teamCaseworkPage.getCheckbox().its('length').should('be.gt', 0);
	
	});


    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    });
});