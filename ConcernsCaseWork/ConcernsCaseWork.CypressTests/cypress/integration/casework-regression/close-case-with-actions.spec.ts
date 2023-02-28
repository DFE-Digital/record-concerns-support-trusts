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
import { ViewClosedCasePage } from "cypress/pages/createCase/viewClosedCasePage";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import actionTable from "cypress/pages/caseRows/caseActionTable";
import { toDisplayDate } from "cypress/support/formatDate";

describe("Testing closing of cases when there are case actions and concerns", () =>
{
    let caseId: string;
    let trustName: string;
    let now: Date;

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

    const viewClosedCasePage = new ViewClosedCasePage();

    beforeEach(() => {
		cy.login();
        now = new Date();

        cy.basicCreateCase()
            .then((id: number) => {
                caseId = id + "";
                return CaseManagementPage.getTrust()
            })
            .then((trust: string) =>
            {
                trustName = trust.trim();
            });
	});

    describe("When we have case actions and concerns that have not been closed", () =>
    {
        it.only("Should raise a validation error for each case action that has not been closed and only allow a case to be closed when they are resolved", () =>
        {
            addAllAllowedCaseActions();

            Logger.Log("Validating an error is displayed for each type of case action")

            CaseManagementPage.getCloseCaseBtn().click();

            CaseManagementPage
                .hasClosedCaseValidationError("Resolve Financial Plan")
                .hasClosedCaseValidationError("Resolve SRMA")
                .hasClosedCaseValidationError("Resolve NTI Under Consideration")
                .hasClosedCaseValidationError("Resolve Decision")
                .hasClosedCaseValidationError("Resolve Concerns");

            CaseManagementPage.getBackBtn().click();

            resolveAllAllowedCaseActions();

            closeConcern();
            closeCaseCheckingValidation();
            verifyClosedCaseDetails();

            Logger.Log("Verifying the closed case action is displayed");
            viewClosedCasePage
                .hasClosedCaseAction("SRMA")
                .hasClosedCaseAction("Financial Plan")
                .hasClosedCaseAction("NTI Under Consideration")
                .hasClosedCaseAction("Decision: No Decision Types")

            Logger.Log("Verifying the closed case actions details are displayed");
            actionTable
                .getRowByAction("SRMA")
                .then((row) =>
                {
                    row
                        .hasStatus("SRMA Canceled")
                        .hasOpenedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                })

            actionTable
                .getRowByAction("Financial Plan")
                .then((row) =>
                {
                    row
                        .hasStatus("Viable plan received")
                        .hasOpenedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                })

            actionTable
                .getRowByAction("NTI Under Consideration")
                .then((row) =>
                {
                    row
                        .hasStatus("No further action being taken")
                        .hasOpenedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                })

            actionTable
                .getRowByAction("Decision: No Decision Types")
                .then((row) =>
                {
                    row
                        .hasStatus("Approved")
                        .hasOpenedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                })
                

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
            CaseManagementPage.hasClosedCaseValidationError("Resolve NTI Warning Letter");
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
            verifyClosedCaseDetails();

            Logger.Log("Verifying the closed case action is displayed");
            viewClosedCasePage
                .hasClosedCaseAction("NTI Warning Letter")

            Logger.Log("Verifying the closed case action is displayed");
            actionTable
                .getRowByAction("NTI Warning Letter")
                .then((row) =>
                {
                    row
                        .hasStatus("Cancelled")
                        .hasOpenedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                })
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
            CaseManagementPage.hasClosedCaseValidationError("Resolve Notice To Improve");
            CaseManagementPage.getBackBtn().click();

            Logger.Log("Completing Notice To Improve");
            cy.get("#open-case-actions td")
                .getByTestId("NTI").click();

            viewNtiPage.close();
            closeNtiPage.close();

            closeConcern();
            closeCase();
            verifyClosedCaseDetails();

            Logger.Log("Verifying the closed case action is displayed");
            viewClosedCasePage
                .hasClosedCaseAction("NTI")
            actionTable
                .getRowByAction("NTI")
                .then((row) =>
                {
                    row
                        .hasStatus("Closed")
                        .hasOpenedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                })
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
            .withDayTrustContacted("05")
            .withMonthTrustContacted("06")
            .withYearTrustContacted("2022")
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

        editSrmaPage
            .withReason("Offer Linked")
            .save();
        
        viewSrmaPage
            .cancel();

        editSrmaPage
            .confirmCancelled()
            .save();

        Logger.Log("Completing Financial Plan");
        cy.get("#open-case-actions td")
            .getByTestId("Financial Plan").click()

        viewFinancialPlanPage.close();
        closeFinancialPlanPage
            .withPlanReceivedDay("05")
            .withPlanReceivedMonth("06")
            .withPlanReceivedYear("2022")
            .withReasonForClosure("Viable Plan Received")
            .close();

        Logger.Log("Completing NTI Under Consideration");
        cy.get("#open-case-actions td")
            .getByTestId("NTI Under Consideration").click();

        viewNtiUnderConsiderationPage.close();
        closeNtiUnderConsiderationPage
            .withStatus("No further action being taken")
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

    function closeCaseCheckingValidation()
    {
        CaseManagementPage.getCaseIDText().then((caseId) =>
        {
            Logger.Log("Closing case");
            CaseManagementPage.getCloseCaseBtn().click();

            Logger.Log("Validating that a rationale for closure must be entered");
            CaseManagementPage.getCloseCaseBtn().click();
            CaseManagementPage.hasValidationError("You have not recorded rationale for closure");
            cy.waitForJavascript();

            Logger.Log("Validating rationale for closure is 200 characters");
            CaseManagementPage.withRationaleForClosureExceedingLimit();
            CaseManagementPage.getCloseCaseBtn().click();
            CaseManagementPage.hasValidationError("Your rationale for closure contains too many characters");

            CaseManagementPage.withRationaleForClosure("Closing case");
            cy.waitForJavascript();
            CaseManagementPage.getCloseCaseBtn().click();

            Logger.Log("Viewing case is closed");
            HomePage.getClosedCasesBtn().click();
            ClosedCasePage.getClosedCase(caseId);
        });
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

    function verifyClosedCaseDetails()
    {
        Logger.Log("Validate Closed Case row has correct details");
        caseworkTable
                .getRowByCaseId(caseId)
                .then((row) =>
                {
                    row
                        .hasCaseId(caseId)
                        .hasCreatedDate(toDisplayDate(now))
                        .hasClosedDate(toDisplayDate(now))
                        .hasTrust(trustName)
                        .hasConcern("Governance and compliance: Compliance")
                })

        Logger.Log("Validate Closed Case has correct details");
        ClosedCasePage.getClosedCase(caseId).click();
        viewClosedCasePage
            .hasConcerns("Governance and compliance: Compliance")
            .hasTerritory("Midlands and West - West Midlands")
            .hasIssue("test")
            .hasCurrentStatus("current status")
            .hasCaseAim("case aim")
            .hasDeEscalationPoint("de-escalation point")
            .hasNextSteps("next steps")
            .hasCaseHistory("case history")
            .hasRationaleForClosure("Closing case");
    }
});