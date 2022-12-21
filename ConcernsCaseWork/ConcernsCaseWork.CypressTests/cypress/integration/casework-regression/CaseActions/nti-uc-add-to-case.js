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

    let term = "";

    it("Should add an NTI under consideration action to a case", () => {

        // Create a new case
        cy.task(LogTask, "Creating a new case")
        cy.checkForExistingCase(true);
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiUnderConsideration');
        AddToCasePage.getCaseActionRadio('NtiUnderConsideration').siblings().should('contain.text', AddToCasePage.actionOptions[9]);
        AddToCasePage.getAddToCaseBtn().click();

        cy.task(LogTask, "User is taken to the correct Case Action page");
        ntiAddPage.getHeadingText().then(term => {
            expect(term.text().trim()).to.match(/NTI: Under consideration/i);
        });

        cy.task(LogTask, "User can add NTI Under Consideration to case");

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
        });

        cy.task(LogTask, "User can click on a link to view a live NTI Under Consideration record");
        CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
            expect($nti).to.be.visible
            expect($nti.text()).to.contain("NTI");
        })
        CaseManagementPage.getOpenActionsTable().should(($status) => {
            expect($status).to.be.visible
        })
        CaseManagementPage.getOpenActionLink("ntiunderconsideration").click();

        cy.task(LogTask, "User can edit Reasons on an existing NTI Under Consideration record");
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

        ntiAddPage.setReasonSelect("random").then(returnedVal =>
        {
            const stText = returnedVal.trim();

            ntiAddPage.getUCAddCaseActionBtn().click();
            utils.getGovErrorSummaryList().should('not.exist');
            ntiAddPage.getNtiTableRow().should(($row) => {
                expect($row.eq(1).text().trim()).to.contain(stText).and.to.match(/Reason/i);
            });
    
            cy.task(LogTask, "No status displayed in the Open Acions table");
            CaseActionsBasePage.getBackToCaseLink().click();
    
            CaseManagementPage.getOpenActionsTable().children().should(($nti) => {
                expect($nti).to.be.visible
                expect($nti.text()).to.contain("NTI");
            })
    
            CaseManagementPage.getOpenActionsTable().should(($status) => {
                expect($status).to.be.visible
                expect($status.text().trim()).to.not.contain(stText);
            })
        });

        cy.task(LogTask, "User can Edit NTI Notes");
        CaseManagementPage.getOpenActionLink("ntiunderconsideration").click();
        ntiAddPage.getEditInformationBtn().click();

        ntiAddPage.getUCAddCaseActionBtn().click();
        ntiAddPage.getNtiTableRow().should(($row) => {
            expect($row.eq(2).text().trim()).to.contain(term.trim()).and.to.match(/Notes/i);
        });

        cy.task(LogTask, "User can close an NTI UC as No Further Action");
        ntiAddPage.getCloseNtiLink().click();
        ntiAddPage.setCloseStatus("random");

        ntiAddPage.setCloseStatus("random").then(returnedVal => {
            const stText = returnedVal.trim();

            cy.log("Logging something " + stText);

            ntiAddPage.getUCAddCaseActionBtn().click();

            CaseManagementPage.getClosedActionsTable().children().should(($nti) => {
                expect($nti).to.be.visible
                expect($nti.text()).to.contain("NTI");
            })

            CaseManagementPage.getClosedActionsTable().children().should(($status) => {
                expect($status.text().trim()).to.contain(stText.trim());
            })
        });
    });

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    });
});
