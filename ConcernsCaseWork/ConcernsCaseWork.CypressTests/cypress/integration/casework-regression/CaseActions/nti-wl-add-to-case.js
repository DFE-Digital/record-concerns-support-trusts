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

    it("Should create a NTI warning letter case action", () => {
        cy.task(LogTask, "Create a case");
        cy.visit(Cypress.env('url'), { timeout: 30000 });
        cy.checkForExistingCase(true);
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiWarningLetter');
        AddToCasePage.getCaseActionRadio('NtiWarningLetter').siblings().should('contain.text', AddToCasePage.actionOptions[10]);
        AddToCasePage.getAddToCaseBtn().click();

        cy.task(LogTask, "User is taken to the correct Case Action page");
        ntiAddPage.getHeadingText().then(term => {
            expect(term.text().trim()).to.match(/NTI: Warning letter/i);
        });

        cy.task(LogTask, "User can add NTI warning letter to case");
        CaseActionsBasePage.setStatusSelect("random")

        //date set
        CaseActionsBasePage.getDateDay().type("10");
        CaseActionsBasePage.getDateMonth().type("6");
        CaseActionsBasePage.getDateYear().type("2023");

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
        });

        ntiAddPage.setConditionSelect("0")
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
            expect($nti.text()).to.contain("NTI Warning Letter");
        })

        cy.task(LogTask, "No status displayed in the Open Acions table");
        CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
            expect($nti.text()).to.contain("NTI Warning Letter");
        });

        cy.task(LogTask, "User is shown validation when adding the same case action");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiWarningLetter');
        AddToCasePage.getAddToCaseBtn().click().then(() => {

            cy.wait(2000).then(() => {

                cy.log(utils.checkForGovErrorSummaryList());

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
        });

        cy.task(LogTask, "User can click on a link to view a live NTI warning letter record");
        CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
            expect($nti.text()).to.contain("NTI Warning Letter");
        });
        CaseManagementPage.getOpenActionsTable().should(($status) => {
            expect($status).to.be.visible
        });
        CaseManagementPage.getOpenActionLink("ntiwarningletter").click();

        ntiAddPage.getHeadingText().should(($heading) => {
            expect($heading.text()).to.contain("NTI: Warning letter");
        });

        cy.task(LogTask, "User can edit an existing NTI warning letter record");
        ntiAddPage.getEditWLInformationBtn().click();

        CaseActionsBasePage.setStatusSelect("1");
        CaseActionsBasePage.getDateDay().clear().type("20");
        CaseActionsBasePage.getDateMonth().clear().type("6");
        CaseActionsBasePage.getDateYear().clear().type("2023");

        ntiAddPage.getAddConditionsBtn().click();

        ntiAddPage.getHeadingText().should(($heading) => {
            expect($heading.text()).to.contain("What are the conditions of the NTI: Warning letter?");
        })

        ntiAddPage.setConditionSelect("1");
        ntiAddPage.setReasonSelect("1");
        ntiAddPage.setReasonSelect("0");

        ntiAddPage.getUpdateConditionsBtn().scrollIntoView().click();

        ntiAddPage.getNotesBox().clear().type("Notes");

        ntiAddPage.getWLAddCaseActionBtn().click();
        utils.getGovErrorSummaryList().should('not.exist');
        CaseManagementPage.getOpenActionLink("ntiwarningletter").click();
        ntiAddPage.getNtiTableRow().should(($row) => {
            expect($row).to.have.length(5);
            expect($row.eq(0).text().trim()).to.contain("Sent to trust").and.to.match(/Current status/i);
            expect($row.eq(1).text().trim()).to.contain("20-06-2023").and.to.match(/(Date sent)/i);
            expect($row.eq(2).text().trim()).to.contain("Cash flow problems").and.to.match(/(Reasons)/i);
            expect($row.eq(3).text().trim()).to.contain("Action plan").and.to.match(/(Conditions)/i);
            expect($row.eq(4).text().trim()).to.contain("Notes").and.to.match(/(Notes)/i);
        });

        cy.task(LogTask, "User can close an nti wl as No Further Action");
        ntiAddPage.getCloseNtiLink().click();
        ntiAddPage.setCloseStatus("random");

        ntiAddPage.setCloseStatus("random").then(returnedVal => 
        {
            const stText = returnedVal.trim();

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
    });

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    });

});
