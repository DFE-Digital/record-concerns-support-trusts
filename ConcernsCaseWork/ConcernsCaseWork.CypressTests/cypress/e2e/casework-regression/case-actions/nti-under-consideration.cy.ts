import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { Logger } from "../../../common/logger";
import { EditNtiUnderConsiderationPage } from "../../../pages/caseActions/ntiUnderConsideration/editNtiUnderConsiderationPage";
import { ViewNtiUnderConsiderationPage } from "../../../pages/caseActions/ntiUnderConsideration/viewNtiUnderConsiderationPage";
import { CloseNtiUnderConsiderationPage } from "../../../pages/caseActions/ntiUnderConsideration/closeNtiUnderConsiderationPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import {NotesError} from "../../../constants/validationErrorConstants";

describe("Testing the NTI under consideration", () =>
{
    const editNtiUnderConsiderationPage = new EditNtiUnderConsiderationPage();
    const viewNtiUnderConsiderationPage = new ViewNtiUnderConsiderationPage();
    const closeNtiUnderConsiderationPage = new CloseNtiUnderConsiderationPage();
    let now: Date;

    beforeEach(() => {
		cy.login();
        now = new Date();

        cy.basicCreateCase();

        addNtiUnderConsiderationToCase();
	});

    describe("When adding or editing a new NTI under consideration", () =>
    {
        it("Should create an NTI with the values specified and be able to edit the values", () =>
        {
            Logger.log("Validating notes field");
            editNtiUnderConsiderationPage
                .withNotesExceedingLimit()
                .save()
                .hasValidationError(NotesError);

            Logger.log("Checking accessibility on Add NTI under consideration");
            cy.excuteAccessibilityTests();

            editNtiUnderConsiderationPage
                .withReason("Cash flow problems")
                .withReason("Safeguarding")
                .withNotes("These are my notes")
                .save();

            Logger.log("Validate the NTI on the view page");
            actionSummaryTable
			.getOpenAction("NTI Under Consideration")
			.then(row =>
			{
				row.hasName("NTI Under Consideration")
				row.hasStatus("In progress")
				row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

            viewNtiUnderConsiderationPage
                .hasReason("Cash flow problems")
                .hasReason("Safeguarding")
                .hasNotes("These are my notes");
                
            viewNtiUnderConsiderationPage.edit();

            editNtiUnderConsiderationPage
                .hasReason("Cash flow problems")
                .hasReason("Safeguarding")
                .hasNotes("These are my notes");

            Logger.log("Validating notes field");
            editNtiUnderConsiderationPage
                .withNotesExceedingLimit()
                .save()
                .hasValidationError(NotesError);

            Logger.log("Checking accessibility on Edit NTI under consideration");
            cy.excuteAccessibilityTests();

            editNtiUnderConsiderationPage
                .clearReasons()
                .withReason("Governance concerns")
                .withReason("Risk of insolvency")
                .withNotes("Edited my notes")
                .save();

            viewNtiUnderConsiderationPage
                .hasDateOpened(toDisplayDate(now))
                .hasReasonCount(2)
                .hasReason("Governance concerns")
                .hasReason("Risk of insolvency")
                .hasNotes("Edited my notes");

            Logger.log("Checking accessibility on View NTI under consideration");
            cy.excuteAccessibilityTests();
        });
    });

    describe("When adding a blank NTI under consideration", () =>
    {
        it("Should set the fields to empty", () =>
        {
            editNtiUnderConsiderationPage
                .save();

            Logger.log("Validate the NTI on the view page");
            actionSummaryTable
			.getOpenAction("NTI Under Consideration")
			.then(row =>
			{
				row.hasStatus("In progress")
				row.select();
			});

            viewNtiUnderConsiderationPage
                .hasDateOpened(toDisplayDate(now))
                .hasReason("Empty")
                .hasNotes("Empty");   
        });
    });

    it("Should only let nti be open per case", () =>
    {
        editNtiUnderConsiderationPage
           .save();

        addNtiUnderConsiderationToCase();

       AddToCasePage
            .hasValidationError("There is already an open NTI action linked to this case. Please resolve that before opening another one.");
    });

    describe("When closing an NTI under consideration", () =>
    {
        it("Should be able to close the NTI under consideration", () =>
        {
            editNtiUnderConsiderationPage
                .withReason("Cash flow problems")
                .withReason("Safeguarding")
                .withNotes("These are my notes")
                .save();

            Logger.log("Close NTI on the view page");
            actionSummaryTable
			.getOpenAction("NTI Under Consideration")
			.then(row =>
			{
				row.select();
			});

            viewNtiUnderConsiderationPage.close();

            closeNtiUnderConsiderationPage.hasNotes("These are my notes");

            Logger.log("Validating the close page")
            closeNtiUnderConsiderationPage
                .withNotesExceedingLimit()
                .close()
                .hasValidationError("Please select a reason for closing NTI under consideration")
                .hasValidationError(NotesError);

            Logger.log("Checking accessibility on Close NTI under consideration");
            cy.excuteAccessibilityTests();

            Logger.log("Filling out the close form");

            closeNtiUnderConsiderationPage
                .withStatus("NoFurtherAction")
                .withNotes("These are my final notes")
                .close();

            Logger.log("Review the closed NTI under consideration");
            actionSummaryTable
			.getClosedAction("NTI Under Consideration")
			.then(row =>
			{
				row.hasName("NTI Under Consideration")
				row.hasStatus("No further action being taken")
				row.hasCreatedDate(toDisplayDate(now))
                row.hasClosedDate(toDisplayDate(now))
				row.select();
			});

            viewNtiUnderConsiderationPage
                .hasDateOpened(toDisplayDate(now))
                .hasDateClosed(toDisplayDate(now))
                .hasReason("Cash flow problems")
                .hasReason("Safeguarding")
                .hasStatus("No further action being taken")
                .hasNotes("These are my final notes")
                .cannotEdit()
                .cannotClose();

            Logger.log("Checking accessibility on View Closed NTI under consideration");
            cy.excuteAccessibilityTests();
        });
    });

    function addNtiUnderConsiderationToCase()
    {
        Logger.log("Adding Notice To Improve");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiUnderConsideration');
        AddToCasePage.getAddToCaseBtn().click();
    }
});