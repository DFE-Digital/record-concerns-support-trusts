import { Logger } from "../../../common/logger";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import "cypress-axe";
import { DeleteCaseGroupClaim } from "cypress/constants/cypressConstants";
import { EditTargetedTrustEngagementPage } from "cypress/pages/caseActions/targetedTrustEngagement/editTargetedTrustEngagementPage";
import { DateIncompleteError, DateInvalidError, NotesError } from "cypress/constants/validationErrorConstants";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { ViewTargetedTrustEngagementPage } from "cypress/pages/caseActions/targetedTrustEngagement/viewTargetedTrustEngagementPage";
import { CloseTargetedTrustEngagementPage } from "cypress/pages/caseActions/targetedTrustEngagement/closeTargetedTrustEngagementPage";
import { DeleteTargetedTrustEngagementPage } from "cypress/pages/caseActions/targetedTrustEngagement/deleteTargetedTrustEngagementPage";

describe("User can add tte to an existing case", () => {
	const viewTargetedTrustEngagementPage = new ViewTargetedTrustEngagementPage();
	const editTargetedTrustEngagementPage = new EditTargetedTrustEngagementPage();
	const closeTargetedTrustEngagementPage = new CloseTargetedTrustEngagementPage();
	const deleteTargetedTrustEngagementPage = new DeleteTargetedTrustEngagementPage();

	let now: Date;
	
	beforeEach(() => {
		cy.login({
			role: DeleteCaseGroupClaim,
		});
		now = new Date();

		cy.basicCreateCase();

		CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('TargetedTrustEngagement');
        AddToCasePage.getAddToCaseBtn().click();
	});

	it("Creating, editing, validating then viewing a targeted trust engagement", function () {

		Logger.log("Validating TTE");
		editTargetedTrustEngagementPage
			.withDateEngagementDay("23")
			.withDateEngagementMonth("25")
			.withDateEngagementYear("2022")
			.save()
			.hasValidationError(
				DateInvalidError.replace("{0}", "Date engagement start")
			);

		editTargetedTrustEngagementPage
			.withDateEngagementDay("23")
			.withDateEngagementMonth("12")
			.withDateEngagementYear("")
			.withNotesExceedingLimit()
			.save()
			.hasValidationError(
				DateIncompleteError.replace("{0}", "Date engagement start")
			)
			.hasValidationError(NotesError);

		editTargetedTrustEngagementPage
			.withDateEngagementDay("04")
			.withDateEngagementMonth("09")
			.withDateEngagementYear("2024")
			.withActivity("Budget Forecast Return/Accounts Return driven")
			.withNotes("Notes")
			.save()
			.hasValidationError("Select Engagement Activity Type")

		Logger.log("Creating TTE");
		editTargetedTrustEngagementPage
			.withDateEngagementDay("04")
			.withDateEngagementMonth("09")
			.withDateEngagementYear("2024")
			.withActivity("Budget Forecast Return/Accounts Return driven")
			.withActivityType("Category 1")
			.withNotes("These are some notes!")
			.save();

		Logger.log("Selecting TTE from open actions");
		actionSummaryTable
			.getOpenAction("TTE - Budget Forecast Return/Accounts Return driven")
			.then(row =>
			{
				row.hasName("TTE - Budget Forecast Return/Accounts Return driven")
				row.hasStatus("In progress")
				row.hasCreatedDate(toDisplayDate(now));
				row.select();
			});

		Logger.log("Viewing TTE");
		viewTargetedTrustEngagementPage
			.hasDateOpened(toDisplayDate(now))
			.hasDateBegan("04 September 2024")
			.hasTypeOfEngagement("Budget Forecast Return/Accounts Return driven")
			.hasTypeOfEngagement("Category 1")
			.hasSupportingNotes("These are some notes!")
			.hasActionEdit()
			.editTTE();

		Logger.log("Editing TTE");

		Logger.log("Check existing values are set");
		editTargetedTrustEngagementPage
			.hasDateEngagementStartDay("04")
			.hasDateEngagementStartMonth("09")
			.hasDateEngagementStartYear("2024")
			.hasActivity("Budget Forecast Return/Accounts Return driven")
			.hasActivityType("Category 1")
			.hasNotes("These are some notes!")

		Logger.log("Set new values");
		editTargetedTrustEngagementPage
			.withDateEngagementDay("05")
			.withDateEngagementMonth("10")
			.withDateEngagementYear("2024")
			.withActivity("Executive pay engagement")
			.withActivityType("CEOs")
			.withNotes("Updated notes!")
			.save()

		Logger.log("Checking accessibility on Edit TTE");
		cy.excuteAccessibilityTests();

		Logger.log("Check the TTE has been updated");
		viewTargetedTrustEngagementPage
			.hasDateOpened(toDisplayDate(now))
			.hasDateBegan("05 October 2024")
			.hasTypeOfEngagement("Executive pay engagement")
			.hasTypeOfEngagement("CEOs")
			.hasSupportingNotes("Updated notes!");
	});

	it("Closing TTE", function () {

		Logger.log("Creating TTE");
		editTargetedTrustEngagementPage
			.withDateEngagementDay("04")
			.withDateEngagementMonth("09")
			.withDateEngagementYear("2024")
			.withActivity("Budget Forecast Return/Accounts Return driven")
			.withActivityType("Category 1")
			.withNotes("These are some notes!")
			.save();

		Logger.log(
			"Selecting the TTE from open cases and validating it before closing it"
		);

		Logger.log("Selecting TTE from open actions");
		actionSummaryTable
			.getOpenAction("TTE - Budget Forecast Return/Accounts Return driven")
			.then(row =>
			{
				row.select();
			});


		Logger.log("Closing the TTTE");
		viewTargetedTrustEngagementPage.closeTTE();

		Logger.log("Validating outcome must be selected & notes can not exceed limits");
		closeTargetedTrustEngagementPage
			.withSupportingNotesExceedingLimit()
			.closeTTE()
			.hasValidationError("Select an outcome for the engagement")
			.hasValidationError("Notes must be 2000 characters or less");

		Logger.log("Checking accessibility on Closed TTE");
		cy.excuteAccessibilityTests();

		Logger.log("Add close TTE finalise supporting notes");
		closeTargetedTrustEngagementPage
			.withDateEngagementEndDay("05")
			.withDateEngagementEndMonth("07")
			.withDateEngagementEndYear("2024")
			.withOutcome("No response required")
			.withFinaliseSupportingNotes("This is a test for closed TTE")
			.closeTTE();

		Logger.log(
			"Selecting TTE from closed cases and verifying that the details matches the above"
		);

		Logger.log("Selecting TTE from closed actions");
		actionSummaryTable
			.getClosedAction("TTE - Budget Forecast Return/Accounts Return driven")
			.then(row =>
			{
				row.hasName("TTE - Budget Forecast Return/Accounts Return driven")
				row.hasStatus("No response required")
				row.hasCreatedDate(toDisplayDate(now))
				row.hasClosedDate(toDisplayDate(now))
				row.select();
			});


		Logger.log("Viewing TTE");
		viewTargetedTrustEngagementPage
			.hasDateOpened(toDisplayDate(now))
			.hasDateClosed(toDisplayDate(now))
			.hasDateBegan("04 September 2024")
			.hasDateEnded("05 July 2024")
			.hasTypeOfEngagement("Budget Forecast Return/Accounts Return driven")
			.hasTypeOfEngagement("Category 1")
			.hasSupportingNotes("This is a test for closed TTE")
			.cannotEditTTE()
			.cannotCloseTTE()

		Logger.log("Checking accessibility on View Closed TTE");
		cy.excuteAccessibilityTests();
	});

	it("Creating and deleting a TTE", function () {

		Logger.log("Creating TTE");
		editTargetedTrustEngagementPage
			.withDateEngagementDay("04")
			.withDateEngagementMonth("09")
			.withDateEngagementYear("2024")
			.withActivity("Budget Forecast Return/Accounts Return driven")
			.withActivityType("Category 1")
			.withNotes("These are some notes!")
			.save();

		Logger.log("Selecting TTE from open actions");
		actionSummaryTable
			.getOpenAction("TTE - Budget Forecast Return/Accounts Return driven")
			.then(row =>
			{
				row.select();
			});

		Logger.log("Deleting TTE");
		viewTargetedTrustEngagementPage
			.deleteTTE();

		deleteTargetedTrustEngagementPage
			.delete();

		Logger.log("Confirm TTE no longer exist");
		actionSummaryTable
			.assertRowDoesNotExist("TTE - Budget Forecast Return/Accounts Return driven", "open");
	});
});
