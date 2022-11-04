import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import ntiAddPage from "/cypress/pages/caseActions/ntiAddPage";
import CaseActionsBasePage from "/cypress/pages/caseActions/caseActionsBasePage";
import { LogTask } from "../../../support/constants";

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



	it("Should add an NTI action to a case", () => {
		cy.task(LogTask, "Case Action already exists");
				cy.visit(Cypress.env('url'), { timeout: 30000 });
				cy.checkForExistingCase(true);
				CaseManagementPage.getAddToCaseBtn().click();
				AddToCasePage.addToCase('Nti');
				AddToCasePage.getCaseActionRadio('Nti').siblings().should('contain.text', AddToCasePage.actionOptions[6].trim());
				AddToCasePage.getAddToCaseBtn().click();
		cy.task(LogTask, "User is taken to the correct Case Action page");
		ntiAddPage.getHeadingText().then(term => {
			expect(term.text().trim()).to.match(/NTI: Notice to improve/i);
		});



		cy.task(LogTask, "User is shown validation for partial date entry");


		//status set
		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseActionsBasePage.setStatusSelect("random")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("stText").then(() => {
					stText = returnedVal;
					cy.log("logging the result " + stText)
				});
				cy.log(self.stText);
				stText = returnedVal;
				cy.log("logging returnedVal " + returnedVal)
			});
		});
		//date set
		CaseActionsBasePage.setDate();

		CaseActionsBasePage.getDateYear().clear();

		ntiAddPage.getWLAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('exist');
		utils.validateGovErorrList('date');

		cy.task(LogTask, "User can add NTI Notice to improve to case");
		//status set
		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseActionsBasePage.setStatusSelect("random")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("stText").then(() => {
					stText = returnedVal;
					cy.log("logging the result " + stText)
				});
				cy.log(self.stText);
				stText = returnedVal;
				cy.log("logging returnedVal " + returnedVal)
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
			expect($heading.text()).to.contain("Conditions for NTI: Notice to improve");
		})
		cy.log("setConditionSelect ").then(() => {
			cy.log("EXECUTED TEST ")
			cy.log(ntiAddPage.setConditionSelect("0")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("condText").then(() => {
					condText = returnedVal;
					cy.log("logging the result " + condText)
				});
				cy.log(self.condText);
				condText = returnedVal;
				cy.log("logging returnedVal " + returnedVal)
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
			expect($nti.text()).to.contain("NTI");
			expect($nti.text()).to.contain(stText.trim());

		})

		cy.task(LogTask, "User is shown validation when adding the same case action");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('Nti');
		AddToCasePage.getAddToCaseBtn().click().then(() => {

			if (utils.checkForGovErrorSummaryList() > 0) {
				cy.log("Case Action already exists").then(() => {
					utils.validateGovErorrList("There is already an open NTI action linked to this case. Please resolve that before opening another one");
				});
			} else {
				cy.log("No Case Action exists");
				cy.log(utils.getGovErrorSummaryList());
			}
			CaseActionsBasePage.getCancelBtn().click();
		});


		cy.task(LogTask, "User can click on a link to view a live NTI record");


		CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
			expect($nti).to.be.visible
			expect($nti.text()).to.contain("NTI");
		})
		CaseManagementPage.getOpenActionsTable().should(($status) => {
			expect($status).to.be.visible
		})
		CaseManagementPage.getOpenActionLink("nti").eq(0).click();

		ntiAddPage.getHeadingText().should(($heading) => {
			expect($heading.text()).to.contain("NTI");
		})


		cy.task(LogTask, "User can edit an existing NTI record");


		ntiAddPage.getEditWLInformationBtn().eq(0).click();
		//status set
		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseActionsBasePage.setStatusSelect("random")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("stText").then(() => {
					stText = returnedVal;
					cy.log("logging the result " + stText)
				});
				cy.log(self.stText);
				cy.log("logging returnedVal " + returnedVal)
			});
		});

		//date set	
		cy.log("setDate ").then(() => {
			cy.log(CaseActionsBasePage.setDate()).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("returnedDate").then(() => {
					returnedDate = returnedVal;
					cy.log("logging returnedDate " + returnedDate);
				});
			});
		});

		ntiAddPage.getAddConditionsBtn().click();

		ntiAddPage.getHeadingText().should(($heading) => {
			expect($heading.text()).to.contain("Conditions for NTI: Notice to improve");
		})

		cy.log("setConditionSelect ").then(() => {
			cy.log("EXECUTED TEST ")
			cy.log(ntiAddPage.setConditionSelect("1")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("condText").then(() => {
					condText = returnedVal;
					cy.log("logging condText " + condText)
				});
				cy.log(self.condText);
				cy.log("logging returnedVal " + returnedVal)
			});
		});

		ntiAddPage.getUpdateConditionsBtn().scrollIntoView().click();


		ntiAddPage.setReasonSelect("1"); //Deselcts		
		cy.log("setReasonSelect ").then(() => {
			cy.log("EXECUTED TEST ")
			cy.log(ntiAddPage.setReasonSelect("0")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("reasText").then(() => {
					reasText = returnedVal;
					cy.log("logging reasText " + reasText)
				});
				cy.log(self.reasText);
				cy.log("logging returnedVal " + returnedVal)
			});
		});
		//notes edit
		cy.log("setNotes ").then(() => {
			cy.log(ntiAddPage.setNotes()).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("notesText").then(() => {
					notesText = returnedVal;
				});
			});
		});


		ntiAddPage.getWLAddCaseActionBtn().eq(0).click();
		utils.getGovErrorSummaryList().should('not.exist');

		CaseManagementPage.getOpenActionLink("nti").eq(0).click();
		ntiAddPage.getNtiTableRow().should(($row) => {
			expect($row).to.have.length(5);
			expect($row.eq(0).text().trim()).to.contain(this.stText.trim()).and.to.match(/Current status/i);
			expect($row.eq(1).text().trim()).to.contain(this.returnedDate.trim()).and.to.match(/(Date NTI Issued)/i);
			expect($row.eq(2).text().trim()).to.contain(this.reasText.trim()).and.to.match(/(Reasons)/i);
			expect($row.eq(3).text().trim()).to.contain(this.condText.trim()).and.to.match(/(Conditions)/i);
			expect($row.eq(4).text().trim()).to.contain(this.notesText).and.to.match(/(Cypress test run)/i);
		});

		cy.task(LogTask, "User can Cancel an existing NTI Notice to improve record");


		//let date = new Date();
		ntiAddPage.getEditWLInformationBtn().eq(1).click();
		CaseActionsBasePage.getHeadingText().should('contain.text', 'Cancel NTI');
		ntiAddPage.getSubHeadingText().should('contain.text', 'Finalise notes');

		//notes edit
		cy.log("setNotes ").then(() => {
			cy.log(ntiAddPage.setNotes()).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("notesText").then(() => {
					notesText = returnedVal;
				});
			});
		});

		ntiAddPage.getCancelNtiBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');

		//Ensures the case action is not prsent in the Open table
		CaseManagementPage.getClosedActionsTable().should('contain.text', 'NTI');
		CaseManagementPage.getClosedActionsTable().should('contain.text', 'Cancelled');
		CaseManagementPage.getClosedActionsTable().should('contain.text', utils.getFormattedDate());


		cy.task(LogTask, "User can Lift an existing NTI Notice to improve record");


		//Recreate NTI

		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('Nti');

		AddToCasePage.getAddToCaseBtn().click();
		cy.wait(2000).then(() => {
			cy.log(utils.checkForGovErrorSummaryList());
			if (utils.checkForGovErrorSummaryList() > 0) {
				cy.log("Case Action already exists");
				cy.visit(Cypress.env('url'), { timeout: 30000 });
				cy.checkForExistingCase(true);
				CaseManagementPage.getAddToCaseBtn().click();
				AddToCasePage.addToCase('Nti');
				AddToCasePage.getCaseActionRadio('Nti').siblings().should('contain.text', AddToCasePage.actionOptions[6].trim());
				AddToCasePage.getAddToCaseBtn().click();
			} else {
				cy.log("No Case Action exists");
				cy.log(utils.checkForGovErrorSummaryList());
			}

		});

		//create New NTI
		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseActionsBasePage.setStatusSelect("random")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("stText").then(() => {
					stText = returnedVal;
					cy.log("logging the result " + stText)
				});
				cy.log(self.stText);
				stText = returnedVal;
				cy.log("logging returnedVal " + returnedVal)
			});
		});
		//date set
		CaseActionsBasePage.setDate();
		ntiAddPage.getWLAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');

		//LIFT
		CaseManagementPage.getOpenActionLink("nti").eq(0).click();
		ntiAddPage.getLiftNtiBtn().click();
		CaseActionsBasePage.getHeadingText().should('contain.text', 'Lift NTI');
		ntiAddPage.getSubHeadingText().should('contain.text', 'Finalise notes');
		CaseActionsBasePage.setDate();
		ntiAddPage.getSubmissionDecisionBox().type("Test ID")

		ntiAddPage.getLiftNoticeToImproveBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');

		//Ensures the case action is not prsent in the Open table
		CaseManagementPage.getClosedActionsTable().should('contain.text', 'NTI');
		CaseManagementPage.getClosedActionsTable().should('contain.text', 'Lifted');
		CaseManagementPage.getClosedActionsTable().should('contain.text', utils.getFormattedDate());

		cy.task(LogTask, "User can Close an existing NTI Notice to improve record");



		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('Nti');

		AddToCasePage.getAddToCaseBtn().click();
		cy.wait(2000).then(() => {
			cy.log(utils.checkForGovErrorSummaryList());
			if (utils.checkForGovErrorSummaryList() > 0) {
				cy.log("Case Action already exists");
				cy.visit(Cypress.env('url'), { timeout: 30000 });
				cy.checkForExistingCase(true);
				CaseManagementPage.getAddToCaseBtn().click();
				AddToCasePage.addToCase('Nti');
				AddToCasePage.getCaseActionRadio('Nti').siblings().should('contain.text', AddToCasePage.actionOptions[6].trim());
				AddToCasePage.getAddToCaseBtn().click();
			} else {
				cy.log("No Case Action exists");
				cy.log(utils.checkForGovErrorSummaryList());
			}

		});
		//create New NTI
		cy.log("setStatusSelect ").then(() => {
			cy.log(CaseActionsBasePage.setStatusSelect("random")).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("stText").then(() => {
					stText = returnedVal;
					cy.log("logging the result " + stText)
				});
				cy.log(self.stText);
				stText = returnedVal;
				cy.log("logging returnedVal " + returnedVal)
			});
		});
		//date set
		CaseActionsBasePage.setDate();

		ntiAddPage.getWLAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');

		//Close
		CaseManagementPage.getOpenActionLink("nti").eq(0).click();
		ntiAddPage.getCloseNtiBtn().click();
		CaseActionsBasePage.getHeadingText().should('contain.text', 'Close NTI');
		CaseManagementPage.getTrustHeadingText().should('contain.text', 'Date NTI closed');
		ntiAddPage.getSubHeadingText().should('contain.text', 'Finalise notes');


		cy.log("setDate ").then(() => {
			cy.log(CaseActionsBasePage.setDate()).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("returnedDate").then(() => {
					returnedDate = returnedVal;
					cy.log("logging the result " + returnedDate)
				});
				cy.log(self.returnedDate);
				returnedDate = returnedVal;
				cy.log("logging returnedVal " + returnedDate)
			});
		});
		//notes edit
		cy.log("setNotes ").then(() => {
			cy.log(ntiAddPage.setNotes()).then((returnedVal) => {
				cy.wrap(returnedVal.trim()).as("notesText").then(() => {
					notesText = returnedVal;
				});
			});
		});

		ntiAddPage.getWLAddCaseActionBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');

		//Ensures the case action is not present in the Open table
		CaseManagementPage.getClosedActionsTable().should('contain.text', 'NTI');
		CaseManagementPage.getClosedActionsTable().should('contain.text', 'Closed');
		CaseManagementPage.getClosedActionsTable().should('contain.text', utils.getFormattedDate());
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
