import { Logger } from "../../../common/logger";
import { EditNtiWarningLetterPage } from "../../../pages/caseActions/ntiWarningLetter/editNtiWarningLetterPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { ViewNtiWarningLetterPage } from "../../../pages/caseActions/ntiWarningLetter/viewNtiWarningLetterPage";
import { CloseNtiWarningLetterPage } from "../../../pages/caseActions/ntiWarningLetter/closeNtiWarningLetterPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { DateIncompleteError, DateInvalidError, NotesError } from "cypress/constants/validationErrorConstants";

describe("Testing the NTI warning letter action", () =>
{
    const editNtiWarningLetterPage = new EditNtiWarningLetterPage();
    const viewNtiWarningLetterPage = new ViewNtiWarningLetterPage();
    const closeNtiWarningLetterPage = new CloseNtiWarningLetterPage();
    let now;

    beforeEach(() => {
		cy.login();
        now = new Date();

        cy.basicCreateCase();

        addNtiWarningLetterToCase();
	});

    it("Should be able to add and edit an NTI warning letter", () =>
    {
        Logger.log("Checking validation");
        editNtiWarningLetterPage
            .withNotesExceedingLimit()
            .withDaySent("22")
            .save()
            .hasValidationError(NotesError)
            .hasValidationError(DateIncompleteError.replace("{0}", "Date warning letter sent"));

        editNtiWarningLetterPage
            .withNotes("This is a test")
            .withMonthSent("20")
            .withYearSent("2022")
            .save()
            .hasValidationError(DateInvalidError.replace("{0}", "Date warning letter sent"));

        Logger.log("Checking accessibility on Add NTI warning letter");
        cy.excuteAccessibilityTests();

        createConfiguredNtiWarningLetter();

		actionSummaryTable
			.getOpenAction("NTI Warning Letter")
			.then(row =>
			{
				row.hasName("NTI Warning Letter")
				row.hasStatus("Sent to trust")
				row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

        viewNtiWarningLetterPage
            .hasStatus("Sent to trust")
            .hasDateSent("22 October 2022")
            .hasReason("Cash flow problems")
            .hasReason("Risk of insolvency")
            .hasCondition("Trust financial plan")
            .hasCondition("Action plan")
            .hasCondition("Lines of accountability")
            .hasCondition("Publishing requirements (compliance with)")
            .hasCondition("Financial returns")
            .hasNotes("This is my notes");

        Logger.log("Editing the NTI warning letter");
        viewNtiWarningLetterPage.edit();

        editNtiWarningLetterPage
            .hasStatus("SentToTrust")
            .hasDaySent("22")
            .hasMonthSent("10")
            .hasYearSent("2022")
            .hasReason("Cash flow problems")
            .hasReason("Risk of insolvency")
            .hasNotes("This is my notes");

        Logger.log("Checking accessibility on Edit NTI warning letter");
        cy.excuteAccessibilityTests();

        editNtiWarningLetterPage
            .clearReasons()
            .withStatus("PreparingWarningLetter")
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

        Logger.log("Validate the NTI warning letter on the view page");
		actionSummaryTable
			.getOpenAction("NTI Warning Letter")
			.then(row =>
			{
				row.hasStatus("Preparing warning letter")
				row.select();
			});

        viewNtiWarningLetterPage
            .hasOpenedDate(toDisplayDate(now))
            .hasStatus("Preparing warning letter")
            .hasDateSent("10 February 2020")
            .hasReasonCount(1)
            .hasReason("Governance concerns")
            .hasConditionCount(1)
            .hasCondition("Scheme of delegation")
            .hasNotes("My edited notes");

        Logger.log("Checking accessibility on View NTI warning letter");
        cy.excuteAccessibilityTests();
    });

    it("Should create an empty NTI warning letter", () =>
    {
        editNtiWarningLetterPage
            .save();

        Logger.log("Validate the NTI warning letter on the view page");
		actionSummaryTable
			.getOpenAction("NTI Warning Letter")
			.then(row =>
			{
				row.hasStatus("In progress")
				row.select();
			});

        viewNtiWarningLetterPage
            .hasOpenedDate(toDisplayDate(now))
            .hasStatus("Empty")
            .hasDateSent("Empty")
            .hasReason("Empty")
            .hasCondition("Financial returns")
            .hasNotes("Empty");
    });

    it("Should only let nti be open per case", () =>
    {
        editNtiWarningLetterPage
           .save();

        addNtiWarningLetterToCase();

       AddToCasePage
            .hasValidationError("There is already an open NTI action linked to this case. Please resolve that before opening another one.");
    });

    it("Should be able to close an NTI warning letter", () =>
    {
        createConfiguredNtiWarningLetter();

        Logger.log("Closing the NTI warning letter");
		actionSummaryTable
			.getOpenAction("NTI Warning Letter")
			.then(row =>
			{
				row.select();
			});

        viewNtiWarningLetterPage.close();

        closeNtiWarningLetterPage
            .hasNotes("This is my notes");

        Logger.log("Validating the close page");
        closeNtiWarningLetterPage
            .withNotesExceedingLimit()
            .close()
            .hasValidationError("Please select a reason for closing the warning letter")
            .hasValidationError(NotesError);

        Logger.log("Checking accessibility on Close NTI warning letter");
        cy.excuteAccessibilityTests();

        closeNtiWarningLetterPage
            .withReason("ConditionsMet")
            .withNotes("This is my final notes")
        
        closeNtiWarningLetterPage.close();

        Logger.log("Validate the Closed NTI warning letter on the view page");
		actionSummaryTable
			.getClosedAction("NTI Warning Letter")
			.then(row =>
			{
				row.hasName("NTI Warning Letter")
				row.hasStatus("Conditions met")
				row.hasCreatedDate(toDisplayDate(now))
                row.hasClosedDate(toDisplayDate(now))
				row.select();
			});

        viewNtiWarningLetterPage
            .hasOpenedDate(toDisplayDate(now))
            .hasClosedDate(toDisplayDate(now))
            .hasStatus("Conditions met")
            .hasDateSent("22 October 2022")
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

        Logger.log("Checking accessibility on View Closed NTI warning letter");
        cy.excuteAccessibilityTests();
    });

    function createConfiguredNtiWarningLetter()
    {
        Logger.log("Filling out the form");
        editNtiWarningLetterPage
            .withStatus("SentToTrust")
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
            .withCondition("Publishing requirements (compliance with)");

        editNtiWarningLetterPage.saveConditions();

        Logger.log("Checking accessibility on NTI warning letter conditions");
        cy.excuteAccessibilityTests();

        editNtiWarningLetterPage.save();
    }

    function addNtiWarningLetterToCase()
    {
        Logger.log("Adding Notice To Improve");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiWarningLetter');
        AddToCasePage.getAddToCaseBtn().click();
    }
});