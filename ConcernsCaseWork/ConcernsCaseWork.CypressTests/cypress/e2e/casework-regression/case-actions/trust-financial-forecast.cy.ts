import { Logger } from "../../../common/logger";
import { EditTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/editTrustFinancialForecastPage";
import { ViewTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/viewTrustFinancialForecastPage";
import { CloseTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/closeTrustFinancialForecastPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import {
	DateIncompleteError,
	DateInvalidError,
	NotesError,
} from "cypress/constants/validationErrorConstants";
import validationComponent from "cypress/pages/validationComponent";

describe("User can add trust financial forecast to an existing case", () => {
	const editTFFPage = new EditTrustFinancialForecastPage();
	const viewTFFPage = new ViewTrustFinancialForecastPage();
	const closeTFFPage = new CloseTrustFinancialForecastPage();
	let now: Date;

	beforeEach(() => {
		cy.login();
		now = new Date();
		cy.basicCreateCase();
		addTFFToCase();
	});

	it("Creation of a TFF", function () {
		Logger.log("Create a TFF with invalid values - Shows validation errors");
		validateAddEdit();

		Logger.log("Create a TFF will all values");
		editTFFPage
			.withForecastingTool("Current year - Spring")
			.withDayReviewHappened("26")
			.withMonthReviewHappened("01")
			.withYearReviewHappened("2023")
			.withDayTrustResponded("27")
			.withMonthTrustResponded("02")
			.withYearTrustResponded("2024")
			.withTrustResponseSatisfactory("Satisfactory")
			.withSRMAOffered("Yes")
			.withNotes("Supporting notes")
			.save();

		Logger.log("Check the action summary");
		actionSummaryTable
			.getOpenAction("TFF (trust financial forecast)")
			.then((row) => {
				row.hasName("TFF (trust financial forecast)");
				row.hasStatus("In progress");
				row.hasCreatedDate(toDisplayDate(now));
				row.select();
			});

		Logger.log("View the created TFF with expected values");
		viewTFFPage
			.hasDateOpened(toDisplayDate(now))
			.hasForecastingTool("Current year - Spring")
			.hasInitialReviewDate("26 January 2023")
			.hasTrustRespondedDate("27 February 2024")
			.hasTrustResponse("Satisfactory")
			.hasSRMABeenOffered("Yes")
			.hasNotes("Supporting notes");

		Logger.log("Checking accessibility on View TFF");
		cy.excuteAccessibilityTests();
	});

	it("Should only let one financial forecast be open per case", () => {
		editTFFPage.save();

		addTFFToCase();

		AddToCasePage.hasValidationError(
			"There is already an open trust financial forecast action linked to this case. Please resolve that before opening another one."
		);
	});

	it("Creating a TFF with empty values", function () {
		Logger.log("Create a TFF with empty values");
		editTFFPage.save();

		Logger.log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("TFF (trust financial forecast)")
			.then((row) => {
				row.select();
			});

		Logger.log("View the created TFF with expected values");
		viewTFFPage
			.hasDateOpened(toDisplayDate(now))
			.hasForecastingTool("Empty")
			.hasInitialReviewDate("Empty")
			.hasTrustRespondedDate("Empty")
			.hasTrustResponse("Empty")
			.hasSRMABeenOffered("Empty")
			.hasNotes("Empty");
	});

	it("Edit a TFF", function () {
		Logger.log("Create a TFF values");
		editTFFPage
			.withForecastingTool("Current year - Spring")
			.withDayReviewHappened("26")
			.withMonthReviewHappened("01")
			.withYearReviewHappened("2023")
			.withDayTrustResponded("27")
			.withMonthTrustResponded("02")
			.withYearTrustResponded("2024")
			.withTrustResponseSatisfactory("Satisfactory")
			.withSRMAOffered("Yes")
			.withNotes("Supporting notes")
			.save();

		Logger.log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("TFF (trust financial forecast)")
			.then((row) => {
				row.select();
			});

		viewTFFPage.edit();

		editTFFPage
			.hasForecastingTool("Current year - Spring")
			.hasDayReviewHappened("26")
			.hasMonthReviewHappened("01")
			.hasYearReviewHappened("2023")
			.hasDayTrustResponded("27")
			.hasMonthTrustResponded("02")
			.hasYearTrustResponded("2024")
			.hasTrustResponseSatisfactory("Satisfactory")
			.hasSRMAOffered("Yes")
			.hasNotes("Supporting notes");

		editTFFPage.clearAllDates();
		validateAddEdit();

		Logger.log("Edit a TFF will all values");
		editTFFPage
			.withForecastingTool("Previous year - Spring")
			.withDayReviewHappened("05")
			.withMonthReviewHappened("07")
			.withYearReviewHappened("2022")
			.withDayTrustResponded("06")
			.withMonthTrustResponded("07")
			.withYearTrustResponded("2024")
			.withTrustResponseSatisfactory("Not satisfactory")
			.withSRMAOffered("No")
			.withNotes("Edited notes")
			.save();

		Logger.log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("TFF (trust financial forecast)")
			.then((row) => {
				row.select();
			});

		Logger.log("Validate the TFF on the view page");
		viewTFFPage
			.hasForecastingTool("Previous year - Spring")
			.hasInitialReviewDate("05 July 2022")
			.hasTrustRespondedDate("06 July 2024")
			.hasTrustResponse("Not satisfactory")
			.hasSRMABeenOffered("No")
			.hasNotes("Edited notes");
	});

	it("Close a TFF", function () {
		Logger.log("Create a TFF with populated values");
		editTFFPage
			.withForecastingTool("Previous year - Autumn")
			.withDayReviewHappened("14")
			.withMonthReviewHappened("02")
			.withYearReviewHappened("2022")
			.withDayTrustResponded("15")
			.withMonthTrustResponded("03")
			.withYearTrustResponded("2024")
			.withTrustResponseSatisfactory("Not satisfactory")
			.withSRMAOffered("No")
			.withNotes("very important notes")
			.save();

		Logger.log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("TFF (trust financial forecast)")
			.then((row) => {
				row.select();
			});

		Logger.log("Closing the trust financial forecast");
		viewTFFPage.close();

		Logger.log("Check previous notes are populated");
		closeTFFPage.hasNotes("very important notes");

		Logger.log("Testing validation on close");
		closeTFFPage
			.withNotesExceedingLimit()
			.close()
			.hasValidationError("Finalise notes must be 2000 characters or less");

		Logger.log("Checking accessibility on Close TFF");
		cy.excuteAccessibilityTests();

		Logger.log("Close with valid values");
		closeTFFPage.withNotes("Even more important notes").close();

		Logger.log("Check the action summary for close");
		actionSummaryTable
			.getClosedAction("TFF (trust financial forecast)")
			.then((row) => {
				row.hasName("TFF (trust financial forecast)");
				row.hasStatus("Completed");
				row.hasCreatedDate(toDisplayDate(now));
				row.hasClosedDate(toDisplayDate(now));
				row.select();
			});

		Logger.log("Check the close trust financial forecast");
		viewTFFPage
			.hasDateOpened(toDisplayDate(now))
			.hasDateClosed(toDisplayDate(now))
			.hasForecastingTool("Previous year - Autumn")
			.hasInitialReviewDate("14 February 2022")
			.hasTrustRespondedDate("15 March 2024")
			.hasTrustResponse("Not satisfactory")
			.hasSRMABeenOffered("No")
			.hasNotes("Even more important notes")
			.cannotEdit()
			.cannotClose();

		Logger.log("Checking accessibility on View Closed TFF");
		cy.excuteAccessibilityTests();
	});

	function addTFFToCase() {
		Logger.log("Adding Trust Financial Forecasr");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("TrustFinancialForecast");
		AddToCasePage.getAddToCaseBtn().click();
	}

	function validateAddEdit() {
		editTFFPage.withDayReviewHappened("90").withDayTrustResponded("270").save();

		// Ensure errors appear in field order
		validationComponent.hasValidationErrorsInOrder([
			DateIncompleteError.replace("{0}", "Date SFSO initial review happened"),
			DateIncompleteError.replace("{0}", "Date trust responded"),
		]);

		editTFFPage
			.withMonthReviewHappened("60")
			.withYearReviewHappened("2023")
			.withMonthTrustResponded("30")
			.withYearTrustResponded("2024")
			.withNotesExceedingLimit()
			.save();

		validationComponent.hasValidationErrorsInOrder([
			DateInvalidError.replace("{0}", "Date SFSO initial review happened"),
			DateInvalidError.replace("{0}", "Date trust responded"),
			NotesError,
		]);

		Logger.log("Checking accessibility on Add/Edit TFF");
		cy.excuteAccessibilityTests();
	}
});
