import { Logger } from "../../../common/logger";
import { EditNoticeToImprovePage } from "../../../pages/caseActions/noticeToImprove/editNoticeToImprovePage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { ViewNoticeToImprovePage } from "../../../pages/caseActions/noticeToImprove/viewNoticeToImprovePage";
import { CancelNoticeToImprovePage } from "../../../pages/caseActions/noticeToImprove/cancelNoticeToImprovePage";
import { LiftNoticeToImprovePage } from "../../../pages/caseActions/noticeToImprove/liftNoticeToImprovePage";
import { CloseNoticeToImprovePage } from "../../../pages/caseActions/noticeToImprove/closeNoticeToImprovePage";

describe("Testing case action NTI", () =>
{
    const editNtiPage = new EditNoticeToImprovePage();
    const viewNtiPage = new ViewNoticeToImprovePage();
    const cancelNtiPage = new CancelNoticeToImprovePage();
    const liftNtiPage = new LiftNoticeToImprovePage();
    const closeNtiPage = new CloseNoticeToImprovePage();

    beforeEach(() => {
		cy.login();

        cy.basicCreateCase();

        addNtiToCase();
	});

    it("Should be able to add a new NTI", () =>
    {
        Logger.Log("Incomplete issue date");
        editNtiPage
            .clearDateFields()
            .withDayIssued("22")
            .save()
            .hasValidationError("Please enter a complete date (DD MM YYYY)");

        Logger.Log("Notes Exceeding allowed limit")
        editNtiPage
            .clearDateFields()
            .withNotesExceedingLimit()
            .save()
            .hasValidationError("Notes must be 2000 characters or less");

        configureNtiWithConditions();

        Logger.Log("Validate the NTI on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage
            .hasStatus("Progress on track")
            .hasDateIssued("22 October 2022")
            .hasReasonIssued("Cash flow problems")
            .hasReasonIssued("Risk of insolvency")
            .hasConditions("Audit and risk committee")
            .hasConditions("Trust financial plan")
            .hasConditions("Action plan")
            .hasConditions("Admissions")
            .hasConditions("Review and update safeguarding policies")
            .hasConditions("Off-payroll payments")
            .hasConditions("Financial returns")
            .hasConditions("Trustee contact details")
            .hasConditions("Qualified Floating Charge (QFC)")
            .hasNotes("Nti notes data");
    });

    it("Should handle editing an existing NTI", () =>
    {
        configureNtiWithConditions();

        Logger.Log("Validate the NTI on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage.edit();

        Logger.Log("Ensure previous values are set");
        editNtiPage
            .hasStatus("Progress on track")
            .hasDayIssued("22")
            .hasMonthIssued("10")
            .hasYearIssued("2022")
            .hasReasonIssued("Cash flow problems")
            .hasReasonIssued("Risk of insolvency")
            .hasNotes("Nti notes data");

        editNtiPage
            .editConditions()
            .hasFinancialManagementConditions("Audit and risk committee")
            .hasFinancialManagementConditions("Trust financial plan")
            .hasGovernanceConditions("Action plan")
            .hasComplianceConditions("Admissions")
            .hasSafeguardingConditions("Review and update safeguarding policies")
            .hasFraudAndIrregularity("Off-payroll payments")
            .hasStandardConditions("Financial returns")
            .hasStandardConditions("Trustee contact details")
            .hasAdditionalFinancialSupportConditions("Qualified Floating Charge (QFC)")
            .cancelConditions();

        Logger.Log("Editing the NTI values");

        editNtiPage
            .withStatus("Issued NTI")
            .withDayIssued("11")
            .withMonthIssued("05")
            .withYearIssued("2021")
            .clearReasonFields()
            .withReasonIssued("Governance concerns")
            .withReasonIssued("Safeguarding")
            .withNotes("Edited notes!");

        editNtiPage
            .editConditions()
            .clearConditions()
            .withFinancialManagementConditions("National deals for schools")
            .withGovernanceConditions("Board meetings")
            .withComplianceConditions("Publishing requirements (compliance with)")
            .withSafeguardingConditions("Appoint trustee with leadership responsibility for safeguarding")
            .withFraudAndIrregularity("Procurement policy")
            .withStandardConditions("Delegated freedoms")
            .withAdditionalFinancialSupportConditions("Move to latest model funding agreement")
            .saveConditions();
            
        editNtiPage.save();

        Logger.Log("Validate the changes on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage
            .hasStatus("Issued NTI")
            .hasDateIssued("11 May 2021")
            .hasReasonIssued("Governance concerns")
            .hasReasonIssued("Safeguarding")
            .hasConditions("National deals for schools")
            .hasConditions("Board meetings")
            .hasConditions("Publishing requirements (compliance with)")
            .hasConditions("Appoint trustee with leadership responsibility for safeguarding")
            .hasConditions("Procurement policy")
            .hasConditions("Delegated freedoms")
            .hasConditions("Move to latest model funding agreement")
            .hasNotes("Edited notes!");
    });

    it("Should handle setting only the default fields", () =>
    {
        Logger.Log("Saving an empty form");
        editNtiPage.save();

        Logger.Log("Validate the NTI on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage
            .hasStatus("Empty")
            .hasDateIssued("Empty")
            .hasReasonIssued("Empty")
            .hasConditions("Empty")
            .hasNotes("Empty");
    });

    it("Should only let one nti be created per case", () => 
    {
        Logger.Log("Configuring nti");
        editNtiPage.save();

        Logger.Log("Try to add second nti to case should result in an error");
        addNtiToCase();

        cy.getByTestId("error-text")
            .should("contain.text", "There is already an open NTI action linked to this case. Please resolve that before opening another one.");
    });

    it("should be able to cancel an NTI", () =>
    {
        configureNtiWithConditions();

        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage.cancel();

        cancelNtiPage.hasNotes("Nti notes data");

        cancelNtiPage
            .withNotesExceedingLimit()
            .cancel()
            .hasValidationError("Notes must be 2000 characters or less");

        cy.wait(500);

        cancelNtiPage
            .withNotes("This is my final notes")
            .cancel();

        assertClosedNti();

        viewNtiPage.hasStatus("Cancelled");
    });

    it("Should be able to lift an NTI", () =>
    {
        configureNtiWithConditions();

        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage.lift();

        liftNtiPage.hasNotes("Nti notes data");

        Logger.Log("Validating fields");
        liftNtiPage
            .withDayLifted("22")
            .lift()
            .hasValidationError("Please enter a complete date (DD MM YYYY)");

        liftNtiPage.clearDateFields();

        liftNtiPage
            .withNotesExceedingLimit()
            .lift()
            .hasValidationError("Notes must be 2000 characters or less");

        Logger.Log("Filling out NTI lifted");
        liftNtiPage
            .withSubmissionDecisionId("123456")
            .withDayLifted("12")
            .withMonthLifted("7")
            .withYearLifted("2005")
            .withNotes("This is my final notes")
            .lift();

        assertClosedNti();

        viewNtiPage.hasStatus("Lifted");
    });

    it("Should be able to close an NTI", () =>
    {
        configureNtiWithConditions();

        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage.close();

        closeNtiPage
            .withDayClosed("22")
            .close()
            .hasValidationError("Please enter a complete date (DD MM YYYY)");

        closeNtiPage.clearDateFields();

        closeNtiPage
            .withNotesExceedingLimit()
            .close()
            .hasValidationError("Notes must be 2000 characters or less");

        closeNtiPage
            .withDayClosed("15")
            .withMonthClosed("12")
            .withYearClosed("2020")
            .withNotes("This is my final notes")
            .close();

        assertClosedNti();

        viewNtiPage
            .hasStatus("Closed")
            .hasDateClosed("15 December 2020");
    });

    function addNtiToCase()
    {
        Logger.Log("Adding Notice To Improve");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Nti')
        AddToCasePage.getAddToCaseBtn().click();
    }

    function configureNtiWithConditions()
    {
        Logger.Log("Filling out the NTI form")
        editNtiPage
            .withStatus("Progress on track")
            .withDayIssued("22")
            .withMonthIssued("10")
            .withYearIssued("2022")
            .withReasonIssued("Cash flow problems")
            .withReasonIssued("Risk of insolvency")
            .withNotes("Nti notes data")

        Logger.Log("Filling out the conditions");
        editNtiPage
            .editConditions()
            .withFinancialManagementConditions("Audit and risk committee")
            .withFinancialManagementConditions("Trust financial plan")
            .withGovernanceConditions("Action plan")
            .withComplianceConditions("Admissions")
            .withSafeguardingConditions("Review and update safeguarding policies")
            .withFraudAndIrregularity("Off-payroll payments")
            .withStandardConditions("Financial returns")
            .withStandardConditions("Trustee contact details")
            .withAdditionalFinancialSupportConditions("Qualified Floating Charge (QFC)")
            .saveConditions();

        editNtiPage.save();
    }

    function assertClosedNti()
    {
        Logger.Log("Viewing the closed NTI");

        cy.get("#close-case-actions td")
            .getByTestId("NTI").click();

            viewNtiPage
                .hasDateIssued("22 October 2022")
                .hasReasonIssued("Cash flow problems")
                .hasReasonIssued("Risk of insolvency")
                .hasConditions("Audit and risk committee")
                .hasConditions("Trust financial plan")
                .hasConditions("Action plan")
                .hasConditions("Admissions")
                .hasConditions("Review and update safeguarding policies")
                .hasConditions("Off-payroll payments")
                .hasConditions("Financial returns")
                .hasConditions("Trustee contact details")
                .hasConditions("Qualified Floating Charge (QFC)")
                .hasNotes("This is my final notes")
                .cannotEdit()
                .cannotClose()
                .cannotCancel()
                .cannotLift();
    }
});