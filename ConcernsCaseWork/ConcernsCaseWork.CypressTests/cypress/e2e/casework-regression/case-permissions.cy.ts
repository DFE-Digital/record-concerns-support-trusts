import { EditFinancialPlanPage } from "cypress/pages/caseActions/financialPlan/editFinancialPlanPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { EditTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/editTrustFinancialForecastPage";
import { ViewTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/viewTrustFinancialForecastPage";
import caseApi from "../../api/caseApi";
import { Logger } from "../../common/logger";
import { DecisionOutcomePage } from "../../pages/caseActions/decision/decisionOutcomePage";
import { EditDecisionPage } from "../../pages/caseActions/decision/editDecisionPage";
import { ViewDecisionPage } from "../../pages/caseActions/decision/viewDecisionPage";
import { ViewFinancialPlanPage } from "../../pages/caseActions/financialPlan/viewFinancialPlanPage";
import { EditNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/editNoticeToImprovePage";
import { ViewNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/viewNoticeToImprovePage";
import { EditNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/editNtiUnderConsiderationPage";
import { ViewNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/viewNtiUnderConsiderationPage";
import { EditNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/editNtiWarningLetterPage";
import { ViewNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/viewNtiWarningLetterPage";
import { EditSrmaPage } from "../../pages/caseActions/srma/editSrmaPage";
import { ViewSrmaPage } from "../../pages/caseActions/srma/viewSrmaPage";
import caseMangementPage from "../../pages/caseMangementPage";

describe("Testing permissions on cases and case actions", () => {

    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();
    const editFinancialPlanPage = new EditFinancialPlanPage();
    const viewFinancialPlanPage = new ViewFinancialPlanPage();
    const editNtiPage = new EditNoticeToImprovePage();
    const viewNtiPage = new ViewNoticeToImprovePage();
    const editNtiWarningLetterPage = new EditNtiWarningLetterPage();
    const viewNtiWarningLetterPage = new ViewNtiWarningLetterPage();
    const editNtiUnderConsiderationPage = new EditNtiUnderConsiderationPage();
    const viewNtiUnderConsiderationPage = new ViewNtiUnderConsiderationPage();
    const editDecisionPage = new EditDecisionPage();
    const viewDecisionPage = new ViewDecisionPage();
    const decisionOutcomePage = new DecisionOutcomePage();
    const editTffPage = new EditTrustFinancialForecastPage();
    const viewTffPage = new ViewTrustFinancialForecastPage();

    let caseId: number;

    beforeEach(() => {
        cy.login();

        cy.basicCreateCase()
        .then((id =>
        {
            caseId = id;
        }));
    });

    it("Should not allow a user to edit a case that they did not create", () => {

        Logger.Log("Check that we can edit if we did create the case");
        caseMangementPage
            .showAllConcernDetails()
            .canEditConcern()
            .canEditRiskToTrust()
            .canEditDirectionOfTravel()
            .canEditTerritory()
            .canEditIssue()
            .canEditCurrentStatus()
            .canEditCaseAim()
            .canEditDeEscalationPoint()
            .canEditNextSteps()
            .canEditCaseHistory()
            .canAddCaseAction();

        Logger.Log("Check that we cannot edit if we did not create the case");
        updateCaseOwner(caseId);

        caseMangementPage
            .showAllConcernDetails()
            .cannotEditConcern()
            .cannotEditRiskToTrust()
            .cannotEditDirectionOfTravel()
            .cannotEditTerritory()
            .cannotEditIssue()
            .cannotEditCurrentStatus()
            .cannotEditCaseAim()
            .cannotEditDeEscalactionPoint()
            .cannotEditNextSteps()
            .cannotEditCaseHistory()
            .cannotAddCaseAction();
    });

    it("Should not allow the user to edit an srma that they did not create", () =>
    {
        Logger.Log("Check that the user can edit an SRMA that they did create");
        caseMangementPage
            .addCaseAction("Srma");

        editSrmaPage
            .withStatus("Trust Considering")
            .withDayTrustContacted("05")
            .withMonthTrustContacted("06")
            .withYearTrustContacted("2022")
            .save();

        cy.get("#open-case-actions td")
            .getByTestId("SRMA").click();

        viewSrmaPage
            .canAddStatus()
            .canAddDateTrustContacted()
            .canAddReason()
            .canAddDateAccepted()
            .canAddDateOfVisit()
            .canAddDateReportSentToTrust()
            .canAddNotes()
            .canCancel()
            .canDecline();

        Logger.Log("Check that the user cannot edit an SRMA that they did not create");
        updateCaseOwner(caseId);

        viewSrmaPage
            .cannotAddStatus()
            .cannotAddDateTrustContacted()
            .cannotAddReason()
            .cannotAddDateAccepted()
            .cannotAddDateOfVisit()
            .cannotAddDateReportSentToTrust()
            .cannotAddNotes()
            .cannotCancel()
            .cannotDecline();
    });

    it("Should not allow the user to edit a financial plan that they did not create", () =>
    {
        Logger.Log("Check that the user can edit an SRMA that they did create");
        caseMangementPage
            .addCaseAction("FinancialPlan");

        editFinancialPlanPage.save();

        cy.get("#open-case-actions td")
            .getByTestId("Financial Plan").click();

        viewFinancialPlanPage
            .canEdit()
            .canClose();

        Logger.Log("Check that the user cannot edit a financial plan that they did not create");
        updateCaseOwner(caseId);

        viewFinancialPlanPage
            .cannotEdit()
            .cannotClose();
    });

    it("Should not allow the user to edit an nti that they did not create", () =>
    {
        Logger.Log("Check that the user can edit an nti that they did create");
        caseMangementPage
            .addCaseAction("Nti");

        editNtiPage.save();

        cy.get("#open-case-actions td")
        .getByTestId("NTI").click();

        viewNtiPage
            .canEdit()
            .canCancel()
            .canClose()
            .canLift();

        Logger.Log("Check that the user cannot edit an nti that they did not create");
        updateCaseOwner(caseId);

        viewNtiPage
            .cannotEdit()
            .cannotCancel()
            .cannotClose()
            .cannotLift();
    });

    it("Should not allow the user to edit an nti warning letter that they did not create", () =>
    {
        Logger.Log("Check that the user can edit an nti warning letter that they did create");
        caseMangementPage
            .addCaseAction("NtiWarningLetter");

        editNtiWarningLetterPage.save();

        cy.get("#open-case-actions td")
            .getByTestId("NTI Warning Letter").click();

        viewNtiWarningLetterPage
            .canEdit()
            .canClose();

        Logger.Log("Check that the user cannot edit an nti warning letter that they did not create");
        updateCaseOwner(caseId);

        viewNtiWarningLetterPage
            .cannotEdit()
            .cannotClose();
    });

    it("Should not allow the user to edit an nti under consideration that they did not create", () =>
    {
        Logger.Log("Check that the user can edit an nti under consideration that they did create");
        caseMangementPage
            .addCaseAction("NtiUnderConsideration");

        editNtiUnderConsiderationPage.save();

        cy.get("#open-case-actions td")
            .getByTestId("NTI Under Consideration").click();

        viewNtiUnderConsiderationPage
            .canEdit()
            .canClose();

        Logger.Log("Check that the user cannot edit an nti under consideration that they did not create");
        updateCaseOwner(caseId);
        
        viewNtiUnderConsiderationPage
            .cannotEdit()
            .cannotClose();
    });

    it("Should not allow the user to edit a decision that they did not create", () =>
    {
        Logger.Log("Check that the user can edit an nti decision that they did create");
        caseMangementPage
            .addCaseAction("Decision");

        editDecisionPage.save();

        cy.get("#open-case-actions td")
            .getByTestId("Decision: No Decision Types").click();

        viewDecisionPage.createDecisionOutcome()
        decisionOutcomePage
            .withDecisionOutcomeStatus("Approved")
            .saveDecisionOutcome();

        cy.get("#open-case-actions td")
            .getByTestId("Decision: No Decision Types").click();

        viewDecisionPage
            .canEditDecision()
            .canEditDecisionOutcome()
            .canCloseDecision();

        Logger.Log("Check that the user cannot edit a decision that they did not create");
        updateCaseOwner(caseId);

        viewDecisionPage
            .cannotEditDecision()
            .cannotEditDecisionOutcome()
            .cannotCloseDecision();
    });

    it("Should not allow the user to edit a trust financial forecast that they did not create", () => {
        Logger.Log("Check that the user can edit a trust financial forecast that they did create");
        caseMangementPage
            .addCaseAction("TrustFinancialForecast");

        editTffPage.save();

        actionSummaryTable
            .getOpenAction("Trust Financial Forecast (TFF)")
            .then((row) =>
            {
                row.select();
            })

        viewTffPage
            .canEdit()
            .canClose();

        Logger.Log("Check that the user cannot edit a trust financial forecast that they did not create");
        updateCaseOwner(caseId);

        viewTffPage
            .cannotEdit()
            .cannotClose();
    });

    function updateCaseOwner(caseId: number) {
        caseApi.get(caseId)
        .then((caseResponse) =>
        {
            caseResponse.createdBy = "Automation.User@education.gov.uk";
            caseApi.patch(caseId, caseResponse);
            cy.reload();
        })
    }
});