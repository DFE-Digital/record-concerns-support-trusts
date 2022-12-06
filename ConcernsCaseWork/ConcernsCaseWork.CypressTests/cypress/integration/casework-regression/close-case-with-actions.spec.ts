import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";
import { EditFinancialPlanPage } from "../../pages/caseActions/financialPlan/editFinancialPlanPage";
import { EditDecisionPage } from "../../pages/caseActions/decision/editDecisionPage";
import { EditSrmaPage } from "../../pages/caseActions/srma/editSrmaPage";
import { EditNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/editNtiUnderConsiderationPage";
import { Logger } from "../../common/logger";
import { ViewSrmaPage } from "../../pages/caseActions/srma/viewSrmaPage";
import { ViewFinancialPlanPage } from "../../pages/caseActions/financialPlan/viewFinancialPlanPage";
import { CloseFinancialPlanPage } from "../../pages/caseActions/financialPlan/closeFinancialPlanPage";
import { ViewNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/viewNtiUnderConsiderationPage";
import { CloseNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/closeNtiUnderConsiderationPage";
import EditConcernPage from "../../pages/editConcernPage";
import HomePage from "../../pages/homePage";
import ClosedCasePage from "../../pages/closedCasePage";
import { EditNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/editNtiWarningLetterPage";
import { CloseNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/closeNtiWarningLetterPage";
import { ViewNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/viewNtiWarningLetterPage";
import { EditNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/editNoticeToImprovePage";
import { ViewNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/viewNoticeToImprovePage";
import { CloseNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/closeNoticeToImprovePage";
import { ViewDecisionPage } from "../../pages/caseActions/decision/viewDecisionPage";
import { CloseDecisionPage } from "../../pages/caseActions/decision/closeDecisionPage";
import { DecisionOutcomePage } from "../../pages/caseActions/decision/decisionOutcomePage";

describe("Testing closing of cases when there are case actions and concerns", () =>
{
    const editFinancialPlanPage = new EditFinancialPlanPage();
    const viewFinancialPlanPage = new ViewFinancialPlanPage();
    const closeFinancialPlanPage = new CloseFinancialPlanPage();

    const editDecisionPage =  new EditDecisionPage();
    const viewDecisionPage = new ViewDecisionPage();
    const closeDecisionPage = new CloseDecisionPage();
    const decisionOutcomePage = new DecisionOutcomePage();

    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();

    const editNtiUnderConsiderationPage = new EditNtiUnderConsiderationPage();
    const viewNtiUnderConsiderationPage = new ViewNtiUnderConsiderationPage();
    const closeNtiUnderConsiderationPage = new CloseNtiUnderConsiderationPage();

    const editNtiWarningLetterPage = new EditNtiWarningLetterPage();
    const closeNtiWarningLetterPage = new CloseNtiWarningLetterPage();
    const viewNtiWarningLetterPage = new ViewNtiWarningLetterPage();

    const editNtiPage = new EditNoticeToImprovePage();
    const viewNtiPage = new ViewNoticeToImprovePage();
    const closeNtiPage =  new CloseNoticeToImprovePage();

    beforeEach(() => {
		cy.login();

        cy.basicCreateCase();
	});

    describe("When we have case actions and concerns that have not been closed", () =>
    {
        it("Should raise a validation error for each case action that has not been closed and only allow a case to be closed when they are resolved", () =>
        {
            addAllAllowedCaseActions();

            Logger.Log("Validating an error is displayed for each type of case action")

            CaseManagementPage.getCloseCaseBtn().click();

            CaseManagementPage
                .hasValidationError("Resolve Financial Plan")
                .hasValidationError("Resolve SRMA")
                .hasValidationError("Resolve NTI Under Consideration")
                .hasValidationError("Resolve Decision")
                .hasValidationError("Resolve Concerns");

            CaseManagementPage.getBackBtn().click();

            resolveAllAllowedCaseActions();

            closeConcern();
            closeCase();
        });

        it("Should raise a validation error for NTI warning letter and only close when the action resolved", () =>
        {
            Logger.Log("Adding NTI warning letter");

            CaseManagementPage.getAddToCaseBtn().click();
            AddToCasePage.addToCase('NtiWarningLetter')
            AddToCasePage.getAddToCaseBtn().click();
            editNtiWarningLetterPage.save();

            Logger.Log("Validating an error is displayed for NTI warning letter when case is closed");
            CaseManagementPage.getCloseCaseBtn().click();
            CaseManagementPage.hasValidationError("Resolve NTI Warning Letter");
            CaseManagementPage.getBackBtn().click();

            Logger.Log("Completing NTI Warning Letter");
            cy.get("#open-case-actions td")
                .getByTestId("NTI Warning Letter").click();

            viewNtiWarningLetterPage.close();
            closeNtiWarningLetterPage
                .withReason("Cancel warning letter")
                .close();

            closeConcern();
            closeCase();
        });

        it("Should raise a validation error for Notice To Improve and only close when the action is resolved", () =>
        {
            Logger.Log("Adding Notice To Improve");
            CaseManagementPage.getAddToCaseBtn().click();
            AddToCasePage.addToCase('Nti')
            AddToCasePage.getAddToCaseBtn().click();
            editNtiPage.save();


            Logger.Log("Validating an error is displayed for Notice To Improve when case is closed");
            CaseManagementPage.getCloseCaseBtn().click();
            CaseManagementPage.hasValidationError("Resolve Notice To Improve");
            CaseManagementPage.getBackBtn().click();

            Logger.Log("Completing Notice To Improve");
            cy.get("#open-case-actions td")
                .getByTestId("NTI").click();

            viewNtiPage.close();
            closeNtiPage.close();

            closeConcern();
            closeCase();
        });
    });

    function addAllAllowedCaseActions()
    {
        Logger.Log("Adding all allowed case actions");

        // Financial plan
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('FinancialPlan')
        AddToCasePage.getAddToCaseBtn().click();
        editFinancialPlanPage.save();

        // Decision
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Decision')
        AddToCasePage.getAddToCaseBtn().click();
        editDecisionPage.save();

        cy.get("#open-case-actions td")
            .getByTestId("Decision: No Decision Types").click();

        viewDecisionPage.createDecisionOutcome()
        decisionOutcomePage
            .withDecisionOutcomeStatus("Approved")
            .saveDecisionOutcome();

        // SRMA
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Srma')
        AddToCasePage.getAddToCaseBtn().click();

        editSrmaPage
            .withStatus("Trust Considering")
            .withContactedDay("05")
            .withContactedMonth("06")
            .withContactedYear("2022")
            .save();

        // NTI warning letter
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiUnderConsideration');
        AddToCasePage.getAddToCaseBtn().click();

        editNtiUnderConsiderationPage.save();
    }

    function resolveAllAllowedCaseActions()
    {
        Logger.Log("Resolving all actions");

        Logger.Log("Completing SRMA");
        cy.get("#open-case-actions td")
            .getByTestId("SRMA").click();

        viewSrmaPage
            .addReason()
            .withSrmaReason("OfferLinked")
            .save()
            .cancelSrma()
            .confirmCancellation()
            .save();

        Logger.Log("Completing Financial Plan");
        cy.get("#open-case-actions td")
            .getByTestId("Financial Plan").click()

        viewFinancialPlanPage.close();
        closeFinancialPlanPage
            .withReasonForClosure("Viable Plan Received")
            .close();

        Logger.Log("Completing NTI Under Consideration");
        cy.get("#open-case-actions td")
            .getByTestId("NTI Under Consideration").click();

        viewNtiUnderConsiderationPage.close();
        closeNtiUnderConsiderationPage
            .withReason("No further action being taken")
            .close();

        Logger.Log("Completing decision");
        cy.get("#open-case-actions td")
            .getByTestId("Decision: No Decision Types").click();

        viewDecisionPage.closeDecision();
        closeDecisionPage.closeDecision();
    }

    function closeConcern()
    {
        Logger.Log("Closing concern");
        CaseManagementPage.editConcern();
        EditConcernPage
            .closeConcern()
            .confirmCloseConcern();
    }

    function closeCase()
    {
        CaseManagementPage.getCaseIDText().then((caseId) =>
        {
            Logger.Log("Closing case");
            CaseManagementPage.getCloseCaseBtn().click();
            CaseManagementPage.withRationaleForClosure("Closing case");
            CaseManagementPage.getCloseCaseBtn().click();

            Logger.Log("Viewing case is closed");
            HomePage.getClosedCasesBtn().click();
            ClosedCasePage.getClosedCase(caseId);
        });
    }
});