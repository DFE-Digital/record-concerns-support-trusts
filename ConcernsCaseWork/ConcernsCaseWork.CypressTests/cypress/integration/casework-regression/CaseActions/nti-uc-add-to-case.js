import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import ntiAddPage from "/cypress/pages/caseActions/ntiAddPage";
import CaseActionsBasePage from "/cypress/pages/caseActions/caseActionsBasePage";

describe("User can add case actions to an existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let term = "";
	let stText = "null";

	it("User enters the case page", () => {
		cy.checkForExistingCase();
	});

	it("User has option to select an NTI: Under consideration Case Action", () => {
		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('NtiUnderConsideration')
		AddToCasePage.getCaseActionRadio('NtiUnderConsideration').siblings().should('contain.text', AddToCasePage.actionOptions[9]);
	});

	it("User can click to add NTI Under Consideration Case Action to a case", function () {
		AddToCasePage.getAddToCaseBtn().click();
		cy.log(utils.checkForGovErrorSummaryList() );

		if (utils.checkForGovErrorSummaryList() > 0 ) { 
			cy.log("Case Action already exists");
					cy.visit(Cypress.env('url'), { timeout: 30000 });
					cy.checkForExistingCase(true);
					CaseManagementPage.getAddToCaseBtn().click();
					AddToCasePage.addToCase('NtiUnderConsideration');
					AddToCasePage.getCaseActionRadio('NtiUnderConsideration').siblings().should('contain.text', AddToCasePage.actionOptions[9]);
					AddToCasePage.getAddToCaseBtn().click();
		}else {
			cy.log("No Case Action exists");	
			cy.log(utils.checkForGovErrorSummaryList() );
		}
	});

	it("User is taken to the correct Case Action page", function () {
		ntiAddPage.getHeadingText().then(term => {
			expect(term.text().trim()).to.match(/NTI: Under consideration/i);
		});
	});

	it("User can add NTI Under Consideration to case", () => {

		ntiAddPage.setReasonSelect("0");
		ntiAddPage.setReasonSelect("1");
		ntiAddPage.setReasonSelect("2");
		ntiAddPage.setReasonSelect("3");
		ntiAddPage.setReasonSelect("4");
		ntiAddPage.setReasonSelect("5");
		ntiAddPage.setReasonSelect("6");
		ntiAddPage.setReasonSelect("7");

		ntiAddPage.getUCAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');

		CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI");
        })
	});

	it("User can click on a link to view a live NTI Under Consideration record", function () {

		CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI");
        })
		CaseManagementPage.getOpenActionsTable().should(($status) => {
			expect($status).to.be.visible
        })
		CaseManagementPage.getOpenActionLink("ntiunderconsideration").click();
//
//>>Add assertion here to validate the redirect was successful
//
	});

	it("User can edit Reasons on an existing NTI Under Consideration record", function () {

		ntiAddPage.getEditInformationBtn().click();

		//Unchecks all the checked reasons
		ntiAddPage.setReasonSelect("0")
		ntiAddPage.setReasonSelect("1")
		ntiAddPage.setReasonSelect("2")
		ntiAddPage.setReasonSelect("3")
		ntiAddPage.setReasonSelect("4")
		ntiAddPage.setReasonSelect("5")
		ntiAddPage.setReasonSelect("6")
		ntiAddPage.setReasonSelect("7")

		cy.log("setReasonSelect ").then(() => {
			cy.log(ntiAddPage.setReasonSelect("random") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log("logging the result "+stText)
				});
				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging returnedVal "+returnedVal)
				});
			});

			ntiAddPage.getUCAddCaseActionBtn().click();
			utils.getGovErrorSummaryList().should('not.exist');
			ntiAddPage.getNtiTableRow().should(($row) => {
				expect($row.eq(0).text().trim()).to.contain(stText.trim()).and.to.match(/Reason/i);
		});
	});

	
	it("No status displayed in the Open Acions table", function () {

		//ntiAddPage.getEditInformationBtn().click();
		CaseActionsBasePage.getBackToCaseLink().click();

		CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI");
        })

		CaseManagementPage.getOpenActionsTable().should(($status) => {
			expect($status).to.be.visible
			expect($status.text().trim()).to.not.contain(this.stText);
        })
	});

	it("User can Edit NTI Notes", function () {

		CaseManagementPage.getOpenActionLink("ntiunderconsideration").click();
		ntiAddPage.getEditInformationBtn().click();

		cy.log("setNotes ").then(() => {
			cy.log(ntiAddPage.setNotes() ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
				});
			});
		});
		ntiAddPage.getUCAddCaseActionBtn().click();
		ntiAddPage.getNtiTableRow().should(($row) => {
			expect($row.eq(1).text().trim()).to.contain(term.trim()).and.to.match(/Notes/i);
		});
	});

	it("User can close an NTI UC as No Further Action", () => {

		ntiAddPage.getCloseNtiLink().click();
		ntiAddPage.setCloseStatus("random");

		cy.log("setCloseStatus ").then(() => {
			cy.log(ntiAddPage.setCloseStatus("random") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log("logging the result "+stText)
				});
				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging returnedVal "+returnedVal)
				});
			});
	
		ntiAddPage.getUCAddCaseActionBtn().click();

			CaseManagementPage.getClosedActionsTable().children().should(($nti) => {
				expect($nti).to.be.visible
				expect($nti.text()).to.contain("NTI");
			})

			CaseManagementPage.getClosedActionsTable().children().should(($status) => {
				expect($status.text().trim()).to.contain(stText.trim());
			})

	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
