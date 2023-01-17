import caseApi from "../../api/caseApi";
import concernsApi from "../../api/concernsApi";
import { Logger } from "../../common/logger";
import { ViewFinancialPlanPage } from "../../pages/caseActions/financialPlan/viewFinancialPlanPage";
import { FinancialPlanPage } from "../../pages/caseActions/financialPlanPage";
import { EditSrmaPage } from "../../pages/caseActions/srma/editSrmaPage";
import { ViewSrmaPage } from "../../pages/caseActions/srma/viewSrmaPage";
import caseMangementPage from "../../pages/caseMangementPage";

describe("Testing permissions on cases and case actions", () => {

    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();
    const editFinancialPlanPage = new FinancialPlanPage();
    const viewFinancialPlanPage = new ViewFinancialPlanPage();

    beforeEach(() => {
        cy.login();

        caseApi.post()
            .then((caseResponse) => {
                const caseId = caseResponse.data.urn;
                concernsApi.post(caseId);

                cy.visit(`/case/${caseId}/management`);
                cy.reload();
            });
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

        viewFinancialPlanPage
            .cannotEdit()
            .cannotClose();
    });
});