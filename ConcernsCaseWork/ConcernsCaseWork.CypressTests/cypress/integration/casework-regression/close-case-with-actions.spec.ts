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

describe("Testing closing of cases when there are case actions and concerns", () =>
{
    const editFinancialPlanPage = new EditFinancialPlanPage();
    const viewFinancialPlanPage = new ViewFinancialPlanPage();
    const closeFinancialPlanPage = new CloseFinancialPlanPage();

    const editDecisionPage =  new EditDecisionPage();
    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();

    const editNtiUnderConsiderationPage = new EditNtiUnderConsiderationPage();
    const viewNtiUnderConsiderationPage = new ViewNtiUnderConsiderationPage();
    const closeNtiUnderConsiderationPage = new CloseNtiUnderConsiderationPage();

    beforeEach(() => {
		cy.login();

        cy.basicCreateCase();
	});

    describe("When we have case actions and concerns that have not been closed", () =>
    {
        it.only("Should raise a validation error for each case action that has not been closed and only allow a case to be closed when they are resolved", () =>
        {
            addAllAllowedCaseActions();

            Logger.Log("Validating an error is displayed for each type of case action")

            CaseManagementPage.getCloseCaseBtn().click();

            CaseManagementPage
                .hasValidationError("Resolve Financial Plan")
                .hasValidationError("Resolve SRMA")
                .hasValidationError("Resolve NTI Under Consideration")
                .hasValidationError("Resolve Concerns");

            CaseManagementPage.getBackBtn().click();

            resolveAllAllowedCaseActions();

            Logger.Log("Closing concern");
            CaseManagementPage.editConcern();
            EditConcernPage
                .closeConcern()
                .confirmCloseConcern();

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
        });
    });

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
    }

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
});