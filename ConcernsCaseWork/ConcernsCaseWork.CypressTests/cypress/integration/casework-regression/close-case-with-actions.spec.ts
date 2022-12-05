import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";
import { EditFinancialPlanPage } from "../../pages/caseActions/financialPlan/editFinancialPlanPage";
import { EditDecisionPage } from "../../pages/caseActions/decision/editDecisionPage";
import { EditSrmaPage } from "../../pages/caseActions/srma/editSrmaPage";
import { EditNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/editntiUnderConsiderationPage";
import { Logger } from "../../common/logger";

describe("Testing closing of cases when there are case actions and concerns", () =>
{
    const editFinancialPlanPage = new EditFinancialPlanPage();
    const editDecisionPage =  new EditDecisionPage();
    const editSrmaPage = new EditSrmaPage();
    const editNtiUnderConsiderationPage = new EditNtiUnderConsiderationPage();

    beforeEach(() => {
		cy.login();

        cy.basicCreateCase();
	});

    describe("When we have case actions and concerns that have not been closed", () =>
    {
        it.only("Should raise a validation error for each case action that has not been closed and only allow a case to be closed when they are resolved", () =>
        {
            addAllAllowedCaseActions();

            CaseManagementPage.getCloseCaseBtn().click();

            CaseManagementPage
                .hasValidationError("Resolve Financial Plan")
                .hasValidationError("Resolve SRMA")
                .hasValidationError("Resolve NTI Under Consideration")
                .hasValidationError("Resolve Concerns");
        });
    });

    describe("When we have case actions and concerns but they have all been closed", () =>
    {
        it("Should allow the case to be closed", () =>
        {
            
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