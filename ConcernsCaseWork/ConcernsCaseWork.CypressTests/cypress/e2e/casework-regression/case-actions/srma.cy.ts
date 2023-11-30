import { Logger } from "../../../common/logger";
import { EditSrmaPage } from "../../../pages/caseActions/srma/editSrmaPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { ViewSrmaPage } from "../../../pages/caseActions/srma/viewSrmaPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import {
	DateIncompleteError,
	DateInvalidError,
	SrmaNotesError,
} from "cypress/constants/validationErrorConstants";
import validationComponent from "cypress/pages/validationComponent";

describe("Testing the SRMA case action", () => {
	const editSrmaPage = new EditSrmaPage();
	const viewSrmaPage = new ViewSrmaPage();
	let now: Date;

	beforeEach(() => {
		cy.login();
		now = new Date();

		cy.basicCreateCase();

		addSrmaToCase();
	});

	it("Should create an SRMA action", () => {
		Logger.log("Checking SRMA validation");
		editSrmaPage
			.withNotesExceedingLimit()
			.save()
			.hasValidationError("Select SRMA status")
			.hasValidationError("Enter date trust was contacted")
			.hasValidationError(SrmaNotesError);

		Logger.log("Checking accessibility on Add SRMA");
		cy.excuteAccessibilityTests();

		Logger.log("Filling out the SRMA form");
		editSrmaPage
			.withStatus("TrustConsidering")
			.withDayTrustContacted("22")
			.withMonthTrustContacted("10")
			.withYearTrustContacted("2022")
			.withNotes("This is my notes")
			.save();

		Logger.log("Add optional SRMA fields on the view page");
		actionSummaryTable.getOpenAction("SRMA").then((row) => {
			row.hasName("SRMA");
			row.hasStatus("Trust considering");
			row.hasCreatedDate(toDisplayDate(now));
			row.select();
		});

		Logger.log("Checking accessibility on View SRMA");
		cy.excuteAccessibilityTests();

		Logger.log("Configure reason");

		viewSrmaPage.addReason();
		editSrmaPage.save().hasValidationError("Select SRMA reason");

		Logger.log("Checking accessibility on Add SRMA Reason");
		cy.excuteAccessibilityTests();

		editSrmaPage.withReason("Regions Group Intervention").save();

		Logger.log("Configure date accepted");
		viewSrmaPage.addDateAccepted();

		editSrmaPage
			.withDayAccepted("22")
			.save()
			.hasValidationError(DateIncompleteError.replace("{0}", "Date accepted"));

		editSrmaPage
			.withMonthAccepted("22")
			.withYearAccepted("2022")
			.save()
			.hasValidationError(DateInvalidError.replace("{0}", "Date accepted"));

		Logger.log("Checking accessibility on Add SRMA Date accepted");
		cy.excuteAccessibilityTests();

		editSrmaPage
			.withDayAccepted("22")
			.withMonthAccepted("05")
			.withYearAccepted("2020")
			.save();

		Logger.log("Configure date of visit");
		viewSrmaPage.addDateOfVisit();

		editSrmaPage.withStartDayOfVisit("22").withEndDayOfVisit("11").save();

		validationComponent.hasValidationErrorsInOrder([
			DateIncompleteError.replace("{0}", "Start date"),
			DateIncompleteError.replace("{0}", "End date"),
		]);

		editSrmaPage
			.withStartMonthOfVisit("22")
			.withStartYearOfVisit("2022")
			.withEndMonthOfVisit("33")
			.withEndYearOfVisit("2021")
			.save();

		validationComponent.hasValidationErrorsInOrder([
			DateInvalidError.replace("{0}", "Start date"),
			DateInvalidError.replace("{0}", "End date"),
		]);

		Logger.log("Checking accessibility on Add Dates of visit");
		cy.excuteAccessibilityTests();

		setValidStartDateOfVisit();

		editSrmaPage
			.withEndDayOfVisit("15")
			.withEndMonthOfVisit("01")
			.withEndYearOfVisit("2021")
			.save();

		validationComponent.hasValidationErrorsInOrder([
			"Start date must be the same as or come before the end date",
			"End date must be the same as or come after the start date",
		]);

		editSrmaPage
			.withEndDayOfVisit("15")
			.withEndMonthOfVisit("08")
			.withEndYearOfVisit("2021")
			.save();

		Logger.log("Configuring date report sent to trust");
		viewSrmaPage.addDateReportSentToTrust();

		editSrmaPage
			.withDayReportSentToTrust("22")
			.save()
			.hasValidationError(
				DateIncompleteError.replace("{0}", "Date report sent")
			);

		editSrmaPage
			.withDayReportSentToTrust("05")
			.withMonthReportSentToTrust("44")
			.withYearReportSentToTrust("2021")
			.save()
			.hasValidationError(DateInvalidError.replace("{0}", "Date report sent"));

		Logger.log("Checking accessibility on Add SRMA Date report sent");
		cy.excuteAccessibilityTests();

		editSrmaPage
			.withDayReportSentToTrust("05")
			.withMonthReportSentToTrust("12")
			.withYearReportSentToTrust("2021")
			.save();

		viewSrmaPage
			.hasDateOpened(toDisplayDate(now))
			.hasStatus("Trust considering")
			.hasDateTrustContacted("22 October 2022")
			.hasReason("Regions Group (RG) action")
			.hasDateAccepted("22 May 2020")
			.hasDateOfVisit("22 July 2021 - 15 August 2021")
			.hasDateReportSentToTrust("05 December 2021")
			.hasNotes("This is my notes");

		Logger.log("Checking accessibility on View SRMA");
		cy.excuteAccessibilityTests();
	});

	it("Should configure an empty SRMA", () => {
		editSrmaPage
			.withStatus("TrustConsidering")
			.withDayTrustContacted("22")
			.withMonthTrustContacted("10")
			.withYearTrustContacted("2022")
			.save();

		actionSummaryTable.getOpenAction("SRMA").then((row) => {
			row.hasStatus("Trust considering");
			row.hasCreatedDate(toDisplayDate(now));
			row.select();
		});

		Logger.log("Check the individual edit pages can handle empty inputs");
		viewSrmaPage.addDateAccepted();
		editSrmaPage.save();

		viewSrmaPage.addDateOfVisit();
		editSrmaPage.save();

		viewSrmaPage.addDateReportSentToTrust();
		editSrmaPage.save();

		viewSrmaPage.addNotes();
		editSrmaPage.save();

		viewSrmaPage
			.hasDateOpened(toDisplayDate(now))
			.hasStatus("Trust considering")
			.hasDateTrustContacted("22 October 2022")
			.hasReason("Empty")
			.hasDateAccepted("Empty")
			.hasDateOfVisit("Empty - Empty")
			.hasDateReportSentToTrust("Empty")
			.hasNotes("Empty");
	});

	it("Should only let one srma be open per case", () => {
		editSrmaPage
			.withStatus("TrustConsidering")
			.withDayTrustContacted("22")
			.withMonthTrustContacted("10")
			.withYearTrustContacted("2022")
			.withNotes("This is my notes")
			.save();

		addSrmaToCase();

		AddToCasePage.hasValidationError(
			"There is already an open SRMA action linked to this case. Please resolve that before opening another one."
		);
	});

	it("Should edit an existing configured SRMA", () => {
		partiallyConfigureSrma("TrustConsidering");

		completeSrmaConfiguration();

		viewSrmaPage.addStatus();
		editSrmaPage.hasStatus("TrustConsidering");
		editSrmaPage.withStatus("Deployed").save();

		viewSrmaPage.addDateTrustContacted();
		editSrmaPage
			.hasDayTrustContacted("22")
			.hasMonthTrustContacted("10")
			.hasYearTrustContacted("2022");

		editSrmaPage
			.clearDateTrustContacted()
			.save()
			.hasValidationError("Enter date trust was contacted");

		editSrmaPage
			.withDayTrustContacted("11")
			.save()
			.hasValidationError(
				DateIncompleteError.replace("{0}", "Date trust was contacted")
			);

		editSrmaPage
			.withDayTrustContacted("11")
			.withMonthTrustContacted("22")
			.withYearTrustContacted("2021")
			.save()
			.hasValidationError(
				DateInvalidError.replace("{0}", "Date trust was contacted")
			);

		editSrmaPage.withMonthTrustContacted("05").save();

		viewSrmaPage.addReason();
		editSrmaPage.hasReason("Regions Group Intervention");
		editSrmaPage.withReason("Offer Linked").save();

		viewSrmaPage.addDateAccepted();
		editSrmaPage
			.hasDayAccepted("22")
			.hasMonthAccepted("05")
			.hasYearAccepted("2020");

		editSrmaPage
			.withDayAccepted("17")
			.withMonthAccepted("06")
			.withYearAccepted("2021")
			.save();

		viewSrmaPage.addDateOfVisit();

		editSrmaPage
			.hasStartDayOfVisit("22")
			.hasStartMonthOfVisit("07")
			.hasStartYearOfVisit("2021")
			.hasEndDayOfVisit("15")
			.hasEndMonthOfVisit("08")
			.hasEndYearOfVisit("2021");

		editSrmaPage
			.withStartDayOfVisit("23")
			.withStartMonthOfVisit("09")
			.withStartYearOfVisit("2022")
			.withEndDayOfVisit("27")
			.withEndMonthOfVisit("10")
			.withEndYearOfVisit("2022")
			.save();

		viewSrmaPage.addDateReportSentToTrust();
		editSrmaPage
			.hasDayReportSentToTrust("05")
			.hasMonthReportSentToTrust("12")
			.hasYearReportSentToTrust("2021");

		editSrmaPage
			.withDayReportSentToTrust("16")
			.withMonthReportSentToTrust("08")
			.withYearReportSentToTrust("2022")
			.save();

		viewSrmaPage.addNotes();

		editSrmaPage.hasNotes("This is my notes");

		editSrmaPage
			.withNotesExceedingLimit()
			.save()
			.hasValidationError(SrmaNotesError);

		Logger.log("Checking accessibility on Add SRMA Notes");
		cy.excuteAccessibilityTests();

		editSrmaPage.withNotes("Editing the notes field").save();

		viewSrmaPage
			.hasStatus("Deployed")
			.hasDateTrustContacted("11 May 2021")
			.hasReason("Offer linked with grant funding or other offer of support")
			.hasDateAccepted("17 June 2021")
			.hasDateOfVisit("23 September 2022 - 27 October 2022")
			.hasDateReportSentToTrust("16 August 2022")
			.hasNotes("Editing the notes field");
	});

	it("Should show correct empty label order dependant on dates of visit", () => {
		partiallyConfigureSrma("Deployed");

		Logger.log("Configure date of visit");
		viewSrmaPage.addDateOfVisit();

		Logger.log("With just a start date");
		editSrmaPage
			.withStartDayOfVisit("05")
			.withStartMonthOfVisit("07")
			.withStartYearOfVisit("2023")
			.save();

		Logger.log("Shows empty label for end date of visit");
		viewSrmaPage.hasDateOfVisit("05 July 2023 - Empty");

		Logger.log("Configure date of visit");
		viewSrmaPage.addDateOfVisit();

		Logger.log("Shows error when end date is entered with no start date");
		editSrmaPage
			.clearDateOfVisit()
			.withEndDayOfVisit("29")
			.withEndMonthOfVisit("03")
			.withEndYearOfVisit("2023")
			.save()
			.hasValidationError("Dates of visit must include a start date");

		Logger.log("Entering valid dates");
		editSrmaPage
			.clearDateOfVisit()
			.withStartDayOfVisit("03")
			.withStartMonthOfVisit("03")
			.withStartYearOfVisit("2023")
			.withEndDayOfVisit("29")
			.withEndMonthOfVisit("03")
			.withEndYearOfVisit("2023")
			.save();

		Logger.log("Shows correct date of visit");
		viewSrmaPage.hasDateOfVisit("03 March 2023 - 29 March 2023");
	});

	describe("Closing an SRMA", () => {
		it("Should be able to resolve an SRMA", () => {
			partiallyConfigureSrma("Deployed");

			Logger.log(
				"Does not allow an SRMA to be resolved without the required fields"
			);

			viewSrmaPage.resolve();

			validationComponent.hasValidationErrorsInOrder([
				"Add reason for SRMA",
				"Enter date trust accepted SRMA",
				"Enter dates of visit",
				"Enter date report sent to trust",
			]);

			completeSrmaConfiguration();

			viewSrmaPage.resolve();

			editSrmaPage.hasNotes("This is my notes");

			editSrmaPage.withNotesExceedingLimit().save();

			validationComponent.hasValidationErrorsInOrder([
				"Confirm SRMA action is complete",
				SrmaNotesError,
			]);

			Logger.log("Checking accessibility on Resolve SRMA");
			cy.excuteAccessibilityTests();

			editSrmaPage.confirmComplete();

			editSrmaPage.withNotes("Resolved notes").save();

			Logger.log("View resolved SRMA");
			actionSummaryTable.getClosedAction("SRMA").then((row) => {
				row.hasName("SRMA");
				row.hasStatus("SRMA complete");
				row.hasCreatedDate(toDisplayDate(now));
				row.hasClosedDate(toDisplayDate(now));
				row.select();
			});

			viewSrmaPage
				.hasDateOpened(toDisplayDate(now))
				.hasDateClosed(toDisplayDate(now))
				.hasStatus("SRMA complete")
				.hasDateTrustContacted("22 October 2022")
				.hasReason("Regions Group (RG) action")
				.hasDateAccepted("22 May 2020")
				.hasDateOfVisit("22 July 2021 - 15 August 2021")
				.hasDateReportSentToTrust("05 December 2021")
				.hasNotes("Resolved notes");

			Logger.log("Checking accessibility on View Closed SRMA");
			cy.excuteAccessibilityTests();
		});

		it("Should cancel an SRMA", () => {
			partiallyConfigureSrma("TrustConsidering");

			viewSrmaPage.cancel().hasValidationError("Add reason for SRMA");

			Logger.log("Configure reason");
			viewSrmaPage.addReason();

			editSrmaPage.withReason("Regions Group Intervention").save();

			viewSrmaPage.cancel();

			editSrmaPage.hasNotes("This is my notes");

			editSrmaPage
				.save()
				.hasValidationError("Confirm SRMA action was cancelled");

			Logger.log("Checking accessibility on Cancel SRMA");
			cy.excuteAccessibilityTests();

			editSrmaPage.confirmCancelled().withNotes("Cancelled notes").save();

			Logger.log("View cancelled SRMA");
			actionSummaryTable.getClosedAction("SRMA").then((row) => {
				row.hasName("SRMA");
				row.hasStatus("SRMA cancelled");
				row.hasCreatedDate(toDisplayDate(now));
				row.hasClosedDate(toDisplayDate(now));
				row.select();
			});

			viewSrmaPage
				.hasDateOpened(toDisplayDate(now))
				.hasDateClosed(toDisplayDate(now))
				.hasStatus("SRMA cancelled")
				.hasDateTrustContacted("22 October 2022")
				.hasReason("Regions Group (RG) action")
				.hasDateAccepted("")
				.hasDateOfVisit("")
				.hasDateReportSentToTrust("")
				.hasNotes("Cancelled notes");
		});

		it("Should decline an SRMA", () => {
			partiallyConfigureSrma("TrustConsidering");

			viewSrmaPage.decline().hasValidationError("Add reason for SRMA");

			Logger.log("Configure reason");
			viewSrmaPage.addReason();

			editSrmaPage.withReason("Regions Group Intervention").save();

			viewSrmaPage.decline();

			editSrmaPage.hasNotes("This is my notes");

			editSrmaPage
				.save()
				.hasValidationError("Confirm SRMA action was declined by trust");

			Logger.log("Checking accessibility on Decline SRMA");
			cy.excuteAccessibilityTests();

			editSrmaPage.confirmDeclined().withNotes("Declined notes").save();

			Logger.log("View declined SRMA");
			actionSummaryTable.getClosedAction("SRMA").then((row) => {
				row.hasName("SRMA");
				row.hasStatus("SRMA declined");
				row.hasCreatedDate(toDisplayDate(now));
				row.hasClosedDate(toDisplayDate(now));
				row.select();
			});

			viewSrmaPage
				.hasDateOpened(toDisplayDate(now))
				.hasDateClosed(toDisplayDate(now))
				.hasStatus("SRMA declined")
				.hasDateTrustContacted("22 October 2022")
				.hasReason("Regions Group (RG) action")
				.hasDateAccepted("")
				.hasDateOfVisit("")
				.hasDateReportSentToTrust("")
				.hasNotes("Declined notes");
		});
	});

	function setValidStartDateOfVisit() {
		editSrmaPage
			.withStartDayOfVisit("22")
			.withStartMonthOfVisit("07")
			.withStartYearOfVisit("2021");
	}

	function addSrmaToCase() {
		Logger.log("Adding Notice To Improve");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("Srma");
		AddToCasePage.getAddToCaseBtn().click();
	}

	function partiallyConfigureSrma(status: string) {
		Logger.log("Filling out the SRMA form");
		editSrmaPage
			.withStatus(status)
			.withDayTrustContacted("22")
			.withMonthTrustContacted("10")
			.withYearTrustContacted("2022")
			.withNotes("This is my notes")
			.save();

		Logger.log("Add optional SRMA fields on the view page");
		actionSummaryTable.getOpenAction("SRMA").then((row) => {
			row.select();
		});
	}

	function completeSrmaConfiguration() {
		Logger.log("Configure reason");
		viewSrmaPage.addReason();
		Logger.log("Verify hint text");
		editSrmaPage.hasNoReasonHintText();
		editSrmaPage.withReason("Regions Group Intervention").save();

		Logger.log("Configure date accepted");
		viewSrmaPage.addDateAccepted();

		editSrmaPage
			.withDayAccepted("22")
			.withMonthAccepted("05")
			.withYearAccepted("2020")
			.save();

		Logger.log("Configure date of visit");
		viewSrmaPage.addDateOfVisit();

		setValidStartDateOfVisit();

		editSrmaPage
			.withEndDayOfVisit("15")
			.withEndMonthOfVisit("08")
			.withEndYearOfVisit("2021")
			.save();

		Logger.log("Configuring date report sent to trust");
		viewSrmaPage.addDateReportSentToTrust();

		editSrmaPage
			.withDayReportSentToTrust("05")
			.withMonthReportSentToTrust("12")
			.withYearReportSentToTrust("2021")
			.save();
	}

	
});
