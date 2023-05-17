import { Logger } from "../../../common/logger";
import { EditTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/editTrustFinancialForecastPage";
import { ViewTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/viewTrustFinancialForecastPage";
import { CloseTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/closeTrustFinancialForecastPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { DateInvalidError } from "cypress/constants/validationErrorConstants";


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
		
		Logger.Log("Create a TFF with invalid values - Shows validation errors");
		editTFFPage
			.withForecastingTool("Current year - Spring")
			.withDayReviewHappened("90")
			.withMonthReviewHappened("60")
			.withYearReviewHappened("2023")
			.withDayTrustResponded("270")
			.withMonthTrustResponded("30")
			.withYearTrustResponded("2024")
			.withNotesExceedingLimit()
			.save()
			.hasValidationError("Supporting notes must be 2000 characters or less")
			.hasValidationError(DateInvalidError.replace("{0}", "When did the trust respond?"))
			.hasValidationError(DateInvalidError.replace("{0}", "When did SFSO initial review happen?"));

		Logger.Log("Checking accessibility on Add TFF");
		cy.excuteAccessibilityTests();

		Logger.Log("Create a TFF will all values");
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

		Logger.Log("Check the action summary");
		actionSummaryTable
			.getOpenAction("Trust Financial Forecast (TFF)")
			.then(row =>
			{
				row.hasName("Trust Financial Forecast (TFF)")
				row.hasStatus("In progress")
				row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

		Logger.Log("View the created TFF with expected values");
		viewTFFPage
			.hasDateOpened(toDisplayDate(now))
			.hasForecastingTool("Current year - Spring")
			.hasInitialReviewDate("26 January 2023")
			.hasTrustRespondedDate("27 February 2024")
			.hasTrustResponse("Satisfactory")
			.hasSRMABeenOffered("Yes")
			.hasNotes("Supporting notes");

			Logger.Log("Checking accessibility on View TFF");
			cy.excuteAccessibilityTests();	
	});

	it("Should only let one financial forecast be open per case", () =>
	{
		editTFFPage
			.save();

		addTFFToCase();

		AddToCasePage
			.hasValidationError("There is already an open trust financial forecast action linked to this case. Please resolve that before opening another one.");
	});

	it("Creating a TFF with empty values", function () {
		Logger.Log("Create a TFF with empty values");
		editTFFPage
			.save();

		Logger.Log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("Trust Financial Forecast (TFF)")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("View the created TFF with expected values");
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
		Logger.Log("Create a TFF values");
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

		Logger.Log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("Trust Financial Forecast (TFF)")
			.then(row =>
			{
				row.select();
			});

		viewTFFPage
			.edit();

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

		Logger.Log("Checking accessibility on Edit TFF");
		cy.excuteAccessibilityTests();	

		Logger.Log("Edit a TFF will all values");
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

		Logger.Log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("Trust Financial Forecast (TFF)")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("Validate the TFF on the view page");
		viewTFFPage
			.hasForecastingTool("Previous year - Spring")
			.hasInitialReviewDate("05 July 2022")
			.hasTrustRespondedDate("06 July 2024")
			.hasTrustResponse("Not satisfactory")
			.hasSRMABeenOffered("No")
			.hasNotes("Edited notes");
	});

	it("Close a TFF", function () {
		Logger.Log("Create a TFF with populated values");
		editTFFPage
			.withForecastingTool("Previous year - Spring")
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

		Logger.Log("Validate the Trust Financial Forecast on the view page");
		actionSummaryTable
			.getOpenAction("Trust Financial Forecast (TFF)")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("Closing the trust financial forecast");
		viewTFFPage.close();

		Logger.Log("Check previous notes are populated")
		closeTFFPage
			.hasNotes("very important notes")

		Logger.Log("Testing validation on close");
		closeTFFPage
			.withNotesExceedingLimit()
			.close()
			.hasValidationError("Finalise notes must be 2000 characters or less");

		Logger.Log("Checking accessibility on Close TFF");
		cy.excuteAccessibilityTests();	

		Logger.Log("Close with valid values");
		closeTFFPage
			.withNotes("Even more important notes")
			.close();

		Logger.Log("Check the action summary for close");
		actionSummaryTable
			.getClosedAction("Trust Financial Forecast (TFF)")
			.then(row =>
			{
				row.hasName("Trust Financial Forecast (TFF)");
				row.hasStatus("Completed");
				row.hasCreatedDate(toDisplayDate(now));
				row.hasClosedDate(toDisplayDate(now));
				row.select();
			});

		Logger.Log("Check the close trust financial forecast");
		viewTFFPage
			.hasDateOpened(toDisplayDate(now))
			.hasDateClosed(toDisplayDate(now))
			.hasForecastingTool("Previous year - Spring")
			.hasInitialReviewDate("14 February 2022")
			.hasTrustRespondedDate("15 March 2024")
			.hasTrustResponse("Not satisfactory")
			.hasSRMABeenOffered("No")
			.hasNotes("Even more important notes")
			.cannotEdit()
			.cannotClose();

		Logger.Log("Checking accessibility on View Closed TFF");
		cy.excuteAccessibilityTests();	
	});

	function addTFFToCase()
    {
        Logger.Log("Adding Trust Financial Forecasr");
        CaseManagementPage.getAddToCaseBtn().click();
       	AddToCasePage.addToCase('TrustFinancialForecast')
    	AddToCasePage.getAddToCaseBtn().click();
    }
});