import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import ntiAddPage from "/cypress/pages/caseActions/ntiAddPage";
import CaseActionsBasePage from "/cypress/pages/caseActions/caseActionsBasePage";
import homePage from "/cypress/pages/homePage";
import closedCasePage from "/cypress/pages/closedCasePage.js";

describe("User can see case actions displayed in closed cases", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let stText = "null";
	let caseid = "null;"


	it("User enters the case page", () => {
		cy.checkForExistingCase();
	});

	it("User has option to select an NTI: warning letter Case Action", () => {
		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('NtiWarningLetter');
		AddToCasePage.getCaseActionRadio('NtiWarningLetter').siblings().should('contain.text', AddToCasePage.actionOptions[10]);
	});

	it("User can click to add NTI warning letter Case Action to a case", function () {
		AddToCasePage.getAddToCaseBtn().click().then(() => {
		cy.log(utils.checkForGovErrorSummaryList() );
		cy.wait(2000).then(() => {

			cy.log(utils.checkForGovErrorSummaryList() );
	
			if (utils.checkForGovErrorSummaryList() > 0 ) { 
			cy.log("Case Action already exists");
					cy.visit(Cypress.env('url'), { timeout: 30000 });
					cy.checkForExistingCase(true);
					CaseManagementPage.getAddToCaseBtn().click();
					AddToCasePage.addToCase('NtiWarningLetter');
					AddToCasePage.getCaseActionRadio('NtiWarningLetter').siblings().should('contain.text', AddToCasePage.actionOptions[10]);
					AddToCasePage.getAddToCaseBtn().click();
		}else {
			cy.log("No Case Action exists");	
			cy.log(utils.checkForGovErrorSummaryList() );
		}
		});
	});
	});

	it("User is taken to the correct Case Action page", function () {
		ntiAddPage.getHeadingText().then(term => {
			expect(term.text().trim()).to.match(/NTI: Warning letter/i);
		});
	});


	it("User can add NTI warning letter to case", () => {
		//status set
		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseActionsBasePage.setStatusSelect("random") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log("logging the result "+stText)
				});
				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging returnedVal "+returnedVal)
				});
			});
		//date set
			CaseActionsBasePage.setDate();
		//reasons set
			ntiAddPage.setReasonSelect("0");
			ntiAddPage.setReasonSelect("1");
			ntiAddPage.setReasonSelect("1");
			ntiAddPage.setReasonSelect("2");
			ntiAddPage.setReasonSelect("2");
			ntiAddPage.setReasonSelect("3");
			ntiAddPage.setReasonSelect("3");
			ntiAddPage.setReasonSelect("4");
			ntiAddPage.setReasonSelect("4");
			ntiAddPage.setReasonSelect("5");
			ntiAddPage.setReasonSelect("5");
			ntiAddPage.setReasonSelect("6");
			ntiAddPage.setReasonSelect("6");
			ntiAddPage.setReasonSelect("7");
			ntiAddPage.setReasonSelect("7");

		//conditions set	
		 	ntiAddPage.getAddConditionsBtn().click();
			ntiAddPage.getHeadingText().should(($heading) => {
				expect($heading.text()).to.contain("What are the conditions of the NTI: Warning letter");
			})
				cy.log("setConditionSelect ").then(() => {
					cy.log("EXECUTED TEST ")
					cy.log(ntiAddPage.setConditionSelect("0") ).then((returnedVal) => { 
						cy.wrap(returnedVal.trim()).as("stText").then(() =>{
							stText  = returnedVal;
							cy.log("logging the result "+stText)
						});
						cy.log(self.stText);
						stText  = returnedVal;
						cy.log("logging returnedVal "+returnedVal)
						});
					});

					ntiAddPage.setReasonSelect("1");

					ntiAddPage.setReasonSelect("2");
					ntiAddPage.setReasonSelect("2");
					ntiAddPage.setReasonSelect("3");
					ntiAddPage.setReasonSelect("3");
					ntiAddPage.setReasonSelect("4");
					ntiAddPage.setReasonSelect("4");
					ntiAddPage.setReasonSelect("5");
					ntiAddPage.setReasonSelect("5");
					ntiAddPage.setReasonSelect("6");
					ntiAddPage.setReasonSelect("6");

		ntiAddPage.getUpdateConditionsBtn().click();
		ntiAddPage.getWLAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');
		CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI: Warning letter");
        })
	});

	it("User can close an nti wl as No Further Action", () => {

		CaseManagementPage.getOpenActionLink("ntiwarningletter").click();

		ntiAddPage.getCloseNtiLink().click();

		cy.log("setCloseStatus ").then(() => {
			cy.log(ntiAddPage.setCloseStatus(0) ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log("logging the result "+stText)
				});
				cy.log(self.stText);
				stText  = returnedVal;
				cy.log("logging returnedVal "+returnedVal)
				});
			});
	
		ntiAddPage.getWLAddCaseActionBtn().click();

			CaseManagementPage.getClosedActionsTable().children().should(($nti) => {
				expect($nti).to.be.visible
				expect($nti.text()).to.contain("NTI");
			})

			switch (stText) {
				case "Cancel warning letter":
					CaseManagementPage.getClosedActionsTable().children().should(($status) => {
						expect($status.text().trim()).to.contain('Cancelled');
					})
					break;
				case "Conditions met":
					CaseManagementPage.getClosedActionsTable().children().should(($status) => {
						expect($status.text().trim()).to.contain('Conditions met');
					})
					break;
				case "Escalate to Notice To Improve":
					CaseManagementPage.getClosedActionsTable().children().should(($status) => {
						expect($status.text().trim()).to.contain('Escalated to Notice To Improve');
					})
					break;
				default:
					cy.log("Could not do the thing");
			}
	});


//////////////////////////////////////////


	it("User can close an open concern", function () {
		cy.closeAllOpenConcerns();

		cy.log("Get CaseID Text").then(() => {
			cy.log(CaseManagementPage.getCaseIDText() ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("caseid").then(() =>{
					caseid  = returnedVal;
					cy.log("logging the result "+caseid)
				});
				cy.log(self.stText);
				cy.log("logging caseid out "+caseid)
				});
			});
	});

	it("User can close an open case", function () {
		cy.get('[href*="/case/' + this.caseid + '/management/closure"]').click();

			cy.wait(2000).then(() => {
	
				cy.log(utils.checkForGovErrorSummaryList() );
		
				if (utils.checkForGovErrorSummaryList() > 0 ) { 
				cy.log("Cannot close case due to case action present");
						cy.visit(Cypress.env('url'), { timeout: 30000 });
						cy.checkForExistingCase(true);
						CaseManagementPage.getAddToCaseBtn().click();
						AddToCasePage.addToCase('NtiWarningLetter');
						AddToCasePage.getCaseActionRadio('NtiWarningLetter').siblings().should('contain.text', AddToCasePage.actionOptions[10]);
						AddToCasePage.getAddToCaseBtn().click();

						cy.get('[href*="/case/' + this.caseid + '/management/closure"]').click();					

			}else {
				cy.log("No Case Action exists");	
			}
		});
		cy.get("#case-outcomes").type("SAMPLE CLOSURE TEXT");
		cy.get(".govuk-button").click();
	});

	it("User can navigate to the closed case page", function () {

			cy.visit(Cypress.env('url'), { timeout: 50000 })
	});

	it("User can see Case Action on the closed case page", function () {
		homePage.getClosedCasesBtn().click().then(()=>{

		closedCasePage.getClosedCase(this.caseid).click();
	});
		CaseManagementPage.getClosedActionsTable().children().should(($nti) => {
			expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI");
		})
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
