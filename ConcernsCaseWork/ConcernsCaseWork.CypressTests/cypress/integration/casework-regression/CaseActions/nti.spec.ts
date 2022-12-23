import { Logger } from "../../../common/logger";
import { EditNoticeToImprovePage } from "../../../pages/caseActions/noticeToImprove/editNoticeToImprovePage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { ViewNoticeToImprovePage } from "../../../pages/caseActions/noticeToImprove/viewNoticeToImprovePage";

describe("Testing case action NTI", () =>
{
    const editNtiPage = new EditNoticeToImprovePage();
    const viewNtiPage = new ViewNoticeToImprovePage();

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

        configureNti();

        Logger.Log("Validate the NTI on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI").click();

        viewNtiPage
            .hasStatus("Progress on track")
            .hasDateIssued("22-10-2022")
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
        configureNti();

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
            .hasDateIssued("11-05-2021")
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

    function addNtiToCase()
    {
        Logger.Log("Adding Notice To Improve");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Nti')
        AddToCasePage.getAddToCaseBtn().click();
    }

    function configureNti()
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
});