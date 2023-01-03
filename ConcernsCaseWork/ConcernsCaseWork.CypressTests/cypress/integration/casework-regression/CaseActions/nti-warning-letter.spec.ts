import { Logger } from "../../../common/logger";
import { EditNtiWarningLetterPage } from "../../../pages/caseActions/ntiWarningLetter/editNtiWarningLetterPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { ViewNtiWarningLetterPage } from "../../../pages/caseActions/ntiWarningLetter/viewNtiWarningLetterPage";
import { CloseNtiWarningLetterPage } from "../../../pages/caseActions/ntiWarningLetter/closeNtiWarningLetterPage";

describe("Testing the NTI warning letter action", () =>
{
    const editNtiWarningLetterPage = new EditNtiWarningLetterPage();
    const viewNtiWarningLetterPage = new ViewNtiWarningLetterPage();
    const closeNtiWarningLetterPage = new CloseNtiWarningLetterPage();

    beforeEach(() => {
		cy.login();

        cy.basicCreateCase();

        addNtiWarningLetterToCase();
	});

    it("Should be able to add and edit an NTI warning letter", () =>
    {
        Logger.Log("Checking validation");
        editNtiWarningLetterPage
            .withNotesExceedingLimit()
            .withDaySent("22")
            .save()
            .hasValidationError("Notes must be 2000 characters or less")
            .hasValidationError("Please enter a complete date (DD MM YYYY)");

        createConfiguredNtiWarningLetter();

        Logger.Log("Validate the NTI warning letter on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI Warning Letter").click();

        viewNtiWarningLetterPage
            .hasStatus("Sent to trust")
            .hasDateSent("22-10-2022")
            .hasReason("Cash flow problems")
            .hasReason("Risk of insolvency")
            .hasCondition("Trust financial plan")
            .hasCondition("Action plan")
            .hasCondition("Lines of accountability")
            .hasCondition("Publishing requirements (compliance with)")
            .hasCondition("Financial returns")
            .hasNotes("This is my notes");

        Logger.Log("Editing the NTI warning letter");
        viewNtiWarningLetterPage.edit();

        editNtiWarningLetterPage
            .hasStatus("Sent to trust")
            .hasDaySent("22")
            .hasMonthSent("10")
            .hasYearSent("2022")
            .hasReason("Cash flow problems")
            .hasReason("Risk of insolvency")
            .hasNotes("This is my notes");

        editNtiWarningLetterPage
            .clearReasons()
            .withStatus("Preparing warning letter")
            .withDaySent("10")
            .withMonthSent("02")
            .withYearSent("2020")
            .withReason("Governance concerns")
            .withNotes("My edited notes");

        editNtiWarningLetterPage
            .editConditions()
            .hasCondition("Trust financial plan")
            .hasCondition("Action plan")
            .hasCondition("Lines of accountability")
            .hasCondition("Publishing requirements (compliance with)")
            .hasCondition("Financial returns");

        editNtiWarningLetterPage
            .clearConditions()
            .withCondition("Scheme of delegation")
            .saveConditions();

        editNtiWarningLetterPage.save();

        Logger.Log("Validate the NTI warning letter on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI Warning Letter").click();

        viewNtiWarningLetterPage
            .hasStatus("Preparing warning letter")
            .hasDateSent("10-02-2020")
            .hasReasonCount(1)
            .hasReason("Governance concerns")
            .hasConditionCount(1)
            .hasCondition("Scheme of delegation")
            .hasNotes("My edited notes");
    });

    it("Should create an empty NTI warning letter", () =>
    {
        editNtiWarningLetterPage
            .save();

        Logger.Log("Validate the NTI warning letter on the view page");
        cy.get("#open-case-actions td")
            .getByTestId("NTI Warning Letter").click();

        viewNtiWarningLetterPage
            .hasStatus("Empty")
            .hasDateSent("Empty")
            .hasReason("Empty")
            .hasCondition("Financial returns")
            .hasNotes("Empty");
    });

    it("Should be able to close an NTI warning letter", () =>
    {
        createConfiguredNtiWarningLetter();

        Logger.Log("Closing the NTI warning letter");
        cy.get("#open-case-actions td")
            .getByTestId("NTI Warning Letter").click();

        viewNtiWarningLetterPage.close();

        closeNtiWarningLetterPage
            .hasNotes("This is my notes");

        Logger.Log("Validating the close page");
        closeNtiWarningLetterPage
            .withNotesExceedingLimit()
            .close()
            .hasValidationError("Please select a reason for closing the Warning letter")
            .hasValidationError("Notes must be 2000 characters or less");

        closeNtiWarningLetterPage
            .withReason("Conditions met")
            .withNotes("This is my final notes")
            .close();

        Logger.Log("Validate the Closed NTI warning letter on the view page");
        cy.get("#close-case-actions td")
            .getByTestId("NTI Warning Letter").click();

        viewNtiWarningLetterPage
            .hasStatus("Conditions met")
            .hasDateSent("22-10-2022")
            .hasReason("Cash flow problems")
            .hasReason("Risk of insolvency")
            .hasCondition("Trust financial plan")
            .hasCondition("Action plan")
            .hasCondition("Lines of accountability")
            .hasCondition("Publishing requirements (compliance with)")
            .hasCondition("Financial returns")
            .hasNotes("This is my final notes")
            .cannotEdit()
            .cannotClose();

    });

    function createConfiguredNtiWarningLetter()
    {
        Logger.Log("Filling out the form");
        editNtiWarningLetterPage
            .withStatus("Sent to trust")
            .withDaySent("22")
            .withMonthSent("10")
            .withYearSent("2022")
            .withReason("Cash flow problems")
            .withReason("Risk of insolvency")
            .withNotes("This is my notes");

        editNtiWarningLetterPage
            .editConditions()
            .withCondition("Trust financial plan")
            .withCondition("Action plan")
            .withCondition("Lines of accountability")
            .withCondition("Publishing requirements (compliance with)")
            .saveConditions();

        editNtiWarningLetterPage.save();
    }

    function addNtiWarningLetterToCase()
    {
        Logger.Log("Adding Notice To Improve");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiWarningLetter');
        AddToCasePage.getAddToCaseBtn().click();
    }
});