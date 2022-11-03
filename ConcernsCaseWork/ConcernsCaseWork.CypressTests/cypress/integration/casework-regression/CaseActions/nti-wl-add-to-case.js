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

	let stText = "null";
	let condText = "null";
	let reasText = "null";
	let returnedDate = "null";
	let notesText = "null";

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

	it("No status displayed in the Open Acions table", function () {
		CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI: Warning letter");
        })
		CaseManagementPage.getOpenActionsTable().should(($status) => {
			expect($status).to.be.visible
			expect($status.text().trim()).to.not.contain(this.stText);
        })
	});

	it("User is shown validation when adding the same case action", function () {

		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('NtiWarningLetter');
		AddToCasePage.getAddToCaseBtn().click().then(() =>{

			cy.wait(2000).then(() => {

				cy.log(utils.checkForGovErrorSummaryList() );
		
				if (utils.checkForGovErrorSummaryList() > 0 ) { 
				cy.log("Case Action already exists").then(() =>{
					utils.validateGovErorrList("There is already an open NTI action linked to this case. Please resolve that before opening another one");
				});	
			}else {
				cy.log("No Case Action exists");	
				cy.log(utils.getGovErrorSummaryList() );
			}
			CaseActionsBasePage.getCancelBtn().click();
		});	
	  });	
	});


	it("User can click on a link to view a live NTI warning letter record", function () {

		CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI: Warning letter");
        })
		CaseManagementPage.getOpenActionsTable().should(($status) => {
			expect($status).to.be.visible
        })
		CaseManagementPage.getOpenActionLink("ntiwarningletter").click();

		ntiAddPage.getHeadingText().should(($heading) => {
			expect($heading.text()).to.contain("NTI: Warning letter");
        })
	});

	it("User can edit an existing NTI warning letter record", function () {

		ntiAddPage.getEditWLInformationBtn().click();
	//status set
		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseActionsBasePage.setStatusSelect("random") ).then((returnedVal) => { 
				cy.wrap(returnedVal.trim()).as("stText").then(() =>{
					stText  = returnedVal;
					cy.log("logging the result "+stText)
				});
				cy.log(self.stText);
				cy.log("logging returnedVal "+returnedVal)
				});
			});

		//date set	
			cy.log("setDate ").then(() => {
				cy.log(CaseActionsBasePage.setDate() ).then((returnedVal) => { 
						cy.wrap(returnedVal.trim()).as("returnedDate").then(() =>{
						returnedDate = returnedVal;
						cy.log("logging returnedDate "+returnedDate);
					});
				});
			});

			ntiAddPage.getAddConditionsBtn().click();

			ntiAddPage.getHeadingText().should(($heading) => {
				expect($heading.text()).to.contain("What are the conditions of the NTI: Warning letter?");
			})

			cy.log("setConditionSelect ").then(() => {
				cy.log("EXECUTED TEST ")
				cy.log(ntiAddPage.setConditionSelect("1") ).then((returnedVal) => { 
					cy.wrap(returnedVal.trim()).as("condText").then(() =>{
						condText  = returnedVal;
						cy.log("logging condText "+condText)
					});
					cy.log(self.condText);
					cy.log("logging returnedVal "+returnedVal)
					});
				});

				ntiAddPage.getUpdateConditionsBtn().scrollIntoView().click();

				ntiAddPage.setReasonSelect("1"); //Deselcts		
				cy.log("setReasonSelect ").then(() => {
					cy.log("EXECUTED TEST ")
					cy.log(ntiAddPage.setReasonSelect("0") ).then((returnedVal) => { 
						cy.wrap(returnedVal.trim()).as("reasText").then(() =>{
							reasText  = returnedVal;
							cy.log("logging reasText "+reasText)
						});
						cy.log(self.reasText);
						cy.log("logging returnedVal "+returnedVal)
						});
					});
						//notes edit
					cy.log("setNotes ").then(() => {
						cy.log(ntiAddPage.setNotes() ).then((returnedVal) => { 
							cy.wrap(returnedVal.trim()).as("notesText").then(() =>{
								notesText  = returnedVal;
							});
						});
					});

		ntiAddPage.getWLAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');
		CaseManagementPage.getOpenActionLink("ntiwarningletter").click();
		ntiAddPage.getNtiTableRow().should(($row) => {
				expect($row).to.have.length(5);
				expect($row.eq(0).text().trim()).to.contain(this.stText.trim()).and.to.match(/Current status/i);
				expect($row.eq(1).text().trim()).to.contain(this.returnedDate.trim()).and.to.match(/(Date sent)/i);
				expect($row.eq(2).text().trim()).to.contain(this.reasText.trim()).and.to.match(/(Reasons)/i);
				expect($row.eq(3).text().trim()).to.contain(this.condText.trim()).and.to.match(/(Conditions)/i);
				expect($row.eq(4).text().trim()).to.contain(this.notesText).and.to.match(/(Notes)/i);
		});

	});

	it("User can close an nti wl as No Further Action", () => {

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

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
