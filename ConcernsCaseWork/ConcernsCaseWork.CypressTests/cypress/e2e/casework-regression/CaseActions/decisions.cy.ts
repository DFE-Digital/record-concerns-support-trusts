import { Logger } from "../../../common/logger";
import { EditDecisionPage } from "../../../pages/caseActions/decision/editDecisionPage";
import { ViewDecisionPage } from "../../../pages/caseActions/decision/viewDecisionPage";
import { CloseDecisionPage } from "../../../pages/caseActions/decision/closeDecisionPage";
import { DecisionOutcomePage } from "../../../pages/caseActions/decision/decisionOutcomePage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import "cypress-axe";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { DateIncompleteError, DateInvalidError, NotesError } from "cypress/constants/validationErrorConstants";

describe("User can add decisions to an existing case", () => {
	const viewDecisionPage = new ViewDecisionPage();
	const editDecisionPage = new EditDecisionPage();
	const closeDecisionPage = new CloseDecisionPage();
	const decisionOutcomePage = new DecisionOutcomePage();

	let now: Date;
	
	beforeEach(() => {
		cy.login();
		now = new Date();

		cy.basicCreateCase();

		CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Decision');
        AddToCasePage.getAddToCaseBtn().click();
	});

	it("Creating, editing, validating then viewing a decision", function () {

		Logger.Log("Validating Decision");

		editDecisionPage
			.withDateESFADay("23")
			.withDateESFAMonth("25")
			.withDateESFAYear("2022")
			.save()
			.hasValidationError(
				DateInvalidError.replace("{0}", "Date ESFA received request")
			);

		editDecisionPage
			.withDateESFADay("23")
			.withDateESFAMonth("12")
			.withDateESFAYear("")
			.withSupportingNotesExceedingLimit()
			.save()
			.hasValidationError(
				DateIncompleteError.replace("{0}", "Date ESFA received request")
			)
			.hasValidationError(NotesError);

		Logger.Log("Checking accessibility on Create Decision");
		cy.excuteAccessibilityTests();

		Logger.Log("Creating Decision");
		editDecisionPage
			.withCrmEnquiry("444")
			.withRetrospectiveRequest("no")
			.withSubmissionRequired("yes")
			.withSubmissionLink("www.gov.uk")
			.withDateESFADay("21")
			.withDateESFAMonth("04")
			.withDateESFAYear("2022")
			.withTypeOfDecision("NoticeToImprove")
			.withTypeOfDecision("Section128")
			.withTotalAmountRequested("£140,000")
			.withSupportingNotes("These are some supporting notes!")
			.save();

		Logger.Log("Selecting Decision from open actions");
		actionSummaryTable
			.getOpenAction("Decision: Multiple Decision Types")
			.then(row =>
			{
				row.hasName("Decision: Multiple Decision Types")
				row.hasStatus("In progress")
				row.hasCreatedDate(toDisplayDate(now));
				row.select();
			});

		Logger.Log("Viewing Decision");
		viewDecisionPage
			.hasDateOpened(toDisplayDate(now))
			.hasCrmEnquiry("444")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("Yes")
			.hasSubmissionLink("www.gov.uk")
			.hasDateESFAReceivedRequest("21 April 2022")
			.hasTotalAmountRequested("£140,000")
			.hasTypeOfDecision("Notice to Improve (NTI)")
			.hasTypeOfDecision("Section 128 (S128)")
			.hasSupportingNotes("These are some supporting notes!")
			.hasActionEdit()
			.cannotCloseDecision()
			.editDecision();

		Logger.Log("Editing Decision");
		editDecisionPage
			.hasCrmEnquiry("444")
			.hasRetrospectiveRequest("no")
			.hasSubmissionRequired("yes")
			.hasSubmissionLink("www.gov.uk")
			.hasDateESFADay("21")
			.hasDateESFAMonth("04")
			.hasDateESFAYear("2022")
			.hasTotalAmountRequested("140,000.00")
			.hasTypeOfDecision("NoticeToImprove")
			.hasTypeOfDecision("Section128")
			.hasSupportingNotes("These are some supporting notes!")

		editDecisionPage
			.withCrmEnquiry("777")
			.withRetrospectiveRequest("no")
			.withSubmissionRequired("yes")
			.withSubmissionLink("www.google.uk")
			.withDateESFADay("22")
			.withDateESFAMonth("03")
			.withDateESFAYear("2022")
			.withTypeOfDecision("QualifiedFloatingCharge")
			.withTotalAmountRequested("£130,000")
			.withSupportingNotes("Testing Supporting Notes")

		Logger.Log("Checking accessibility on Edit Decision");
		cy.excuteAccessibilityTests();

		editDecisionPage
			.save();

		Logger.Log("Viewing Edited Decision");
		viewDecisionPage
			.hasCrmEnquiry("777")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("Yes")
			.hasSubmissionLink("www.google.uk")
			.hasDateESFAReceivedRequest("22 March 2022")
			.hasTotalAmountRequested("£130,000")
			.hasTypeOfDecision("Qualified Floating Charge (QFC)")
			.hasSupportingNotes("Testing Supporting Notes");
	});

	it("Closing decision", function () {

		Logger.Log("Adding note on the decision that will be closing ");
		editDecisionPage
			.withCrmEnquiry("444")
			.withRetrospectiveRequest("no")
			.withSubmissionRequired("yes")
			.withSubmissionLink("www.gov.uk")
			.withDateESFADay("21")
			.withDateESFAMonth("04")
			.withDateESFAYear("2022")
			.withTypeOfDecision("NoticeToImprove")
			.withTypeOfDecision("Section128")
			.withTotalAmountRequested("£140,000")
			.withSupportingNotes("This is a test")
			.save();

		Logger.Log(
			"Selecting the Decision from open cases and validating it before closing it"
		);

		Logger.Log("Selecting Decision from open actions");
		actionSummaryTable
			.getOpenAction("Decision: Multiple Decision Types")
			.then(row =>
			{
				row.select();
			});

		viewDecisionPage.createDecisionOutcome();

		decisionOutcomePage
			.withDecisionOutcomeStatus("ApprovedWithConditions")
			.withTotalAmountApproved("50,000")
			.withDateDecisionMadeDay("24")
			.withDateDecisionMadeMonth("11")
			.withDateDecisionMadeYear("2022")
			.withDecisionTakeEffectDay("11")
			.withDecisionTakeEffectMonth("12")
			.withDecisionTakeEffectYear("2023")
			.withDecisionAuthouriser("DeputyDirector")
			.withBusinessArea("BusinessPartner")
			.withBusinessArea("Capital")
			.withBusinessArea("FinancialProviderMarketOversight")
			.saveDecisionOutcome();

		Logger.Log("Selecting Decision from open actions");
		actionSummaryTable
			.getOpenAction("Decision: Multiple Decision Types")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("Selecting decision outcome, saving and closing decision");
		viewDecisionPage.closeDecision();

		closeDecisionPage.hasFinaliseSupportingNotes("This is a test");

		Logger.Log("Validating notes can not exceed limits");
		closeDecisionPage
			.withSupportingNotesExceedingLimit()
			.closeDecision()
			.hasValidationError("Supporting Notes must be 2000 characters or less");

		Logger.Log("Checking accessibility on Closed Decision");
		cy.excuteAccessibilityTests();

		Logger.Log("Add close decision finalise supporting notes");
		closeDecisionPage
			.withFinaliseSupportingNotes("This is a test for closed decision")
			.closeDecision();

		Logger.Log(
			"Selecting Decision from closed cases and verifying that the finalise note matches the above"
		);

		Logger.Log("Selecting Decision from closed actions");
		actionSummaryTable
			.getClosedAction("Decision: Multiple Decision Types")
			.then(row =>
			{
				row.hasName("Decision: Multiple Decision Types")
				row.hasStatus("Approved with conditions")
				row.hasCreatedDate(toDisplayDate(now))
				row.hasClosedDate(toDisplayDate(now))
				row.select();
			});

		viewDecisionPage
			.hasDateOpened(toDisplayDate(now))
			.hasDateClosed(toDisplayDate(now))
			.hasCrmEnquiry("444")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("Yes")
			.hasSubmissionLink("www.gov.uk")
			.hasDateESFAReceivedRequest("21 April 2022")
			.hasTotalAmountRequested("£140,000")
			.hasTypeOfDecision("Notice to Improve (NTI)")
			.hasTypeOfDecision("Section 128 (S128)")
			.hasSupportingNotes("This is a test for closed decision")
			.hasBusinessArea("Business Partner")
			.hasBusinessArea("Capital")
			.hasBusinessArea("FPMO (Financial Provider Market Oversight)")
			.hasDecisionOutcomeStatus("Approved with conditions")
			.hasMadeDate("24 November 2022")
			.hasEffectiveFromDate("11 December 2023")
			.hasTotalAmountApproved("£50,000")
			.hasAuthoriser("Deputy Director")
			.cannotCreateAnotherDecisionOutcome()
			.cannotCloseDecision()
			.cannotEditDecision()
			.cannotEditDecisionOutcome();

		Logger.Log("Checking accessibility on View Closed Decision");
		cy.excuteAccessibilityTests();
	});

	it("When Decision is empty", function () {
		Logger.Log("Creating Empty Decision");
		editDecisionPage.save();

		Logger.Log("Selecting Decision from open actions");
		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.hasName("Decision: No Decision Types")
				row.hasStatus("In progress")
				row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

		Logger.Log("Create Decision Outcome with only status");
		viewDecisionPage
			.createDecisionOutcome();

		decisionOutcomePage
			.withDecisionOutcomeStatus("Withdrawn")
			.saveDecisionOutcome();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("Viewing Empty Decision");
		viewDecisionPage
			.hasDateOpened(toDisplayDate(now))
			.hasCrmEnquiry("Empty")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("No")
			.hasSubmissionLink("Empty")
			.hasDateESFAReceivedRequest("Empty")
			.hasTotalAmountRequested("£0.00")
			.hasTypeOfDecision("Empty")
			.hasSupportingNotes("Empty")
			.hasDecisionOutcomeStatus("Withdrawn")
			.hasBusinessArea("Empty")
			.hasMadeDate("Empty")
			.hasEffectiveFromDate("Empty")
			.hasTotalAmountApproved("£0.00")
			.hasAuthoriser("Empty");
	});

	it("Create, edit and view a decision outcome, checking validation", () => {
		Logger.Log("Creating Empty Decision");
		editDecisionPage
			.save();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("Creating a decision outcome");
		viewDecisionPage
			.hasNoDecisionOutcome()
			.createDecisionOutcome();

		Logger.Log("Checking when no status is selected");
		decisionOutcomePage
			.saveDecisionOutcome()
			.hasValidationError("Select a decision outcome");

		Logger.Log("Checking an invalid date");
		decisionOutcomePage
			.withDecisionOutcomeStatus("Withdrawn")
			.withDateDecisionMadeDay("24")
			.withDateDecisionMadeMonth("13")
			.withDateDecisionMadeYear("2022")
			.withDecisionTakeEffectDay("12")
			.withDecisionTakeEffectMonth("22")
			.withDecisionTakeEffectYear("2023")
			.saveDecisionOutcome()
			.hasValidationError(DateInvalidError.replace("{0}", "Date decision was made"))
			.hasValidationError(DateInvalidError.replace("{0}", "Date decision takes effect"))

		Logger.Log("Checking accessibility on Add Decision Outcome");
		cy.excuteAccessibilityTests();

		Logger.Log("Checking an incomplete dates");
		decisionOutcomePage
			.withDateDecisionMadeDay("24")
			.withDateDecisionMadeMonth("12")
			.withDateDecisionMadeYear("")
			.withDecisionTakeEffectDay("12")
			.withDecisionTakeEffectMonth("06")
			.withDecisionTakeEffectYear("")
			.saveDecisionOutcome()
			.hasValidationError(DateIncompleteError.replace("{0}", "Date decision was made"))
			.hasValidationError(DateIncompleteError.replace("{0}", "Date decision takes effect"));

		Logger.Log("Create Decision Outcome");
		decisionOutcomePage
			.withDecisionOutcomeStatus("ApprovedWithConditions")
			.withTotalAmountApproved("50,000")
			.withDateDecisionMadeDay("24")
			.withDateDecisionMadeMonth("11")
			.withDateDecisionMadeYear("2022")
			.withDecisionTakeEffectDay("11")
			.withDecisionTakeEffectMonth("12")
			.withDecisionTakeEffectYear("2023")
			.withDecisionAuthouriser("DeputyDirector")
			.withBusinessArea("BusinessPartner")
			.withBusinessArea("Capital")
			.withBusinessArea("FinancialProviderMarketOversight")
			.saveDecisionOutcome();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("View decision outcome")

		viewDecisionPage
			.hasDecisionOutcomeStatus("Approved with conditions")
			.hasTotalAmountApproved("£50,000")
			.hasMadeDate("24 November 2022")
			.hasEffectiveFromDate("11 December 2023")
			.hasBusinessArea("Business Partner")
			.hasBusinessArea("Capital")
			.hasBusinessArea("FPMO (Financial Provider Market Oversight)")
			.hasAuthoriser("Deputy Director")
			.cannotCreateAnotherDecisionOutcome();
		
		Logger.Log("Edit decision outcome")
		viewDecisionPage
			.editDecisionOutcome();

		Logger.Log("Verify Existing Values");
		decisionOutcomePage
			.hasDecisionOutcomeStatus("ApprovedWithConditions")
			.hasTotalAmountApproved("50,000")
			.hasDecisionMadeDay("24")
			.hasDecisionMadeMonth("11")
			.hasDecisionMadeYear("2022")
			.hasDateEffectiveFromDay("11")
			.hasDateEffectiveFromMonth("12")
			.hasDateEffectiveFromYear("2023")
			.hasDecisionAuthouriser("DeputyDirector")
			.hasBusinessArea("BusinessPartner")
			.hasBusinessArea("Capital")
			.hasBusinessArea("FinancialProviderMarketOversight");

		Logger.Log("Checking accessibility on Edit Decision Outcome");
		cy.excuteAccessibilityTests();

		Logger.Log("Edit Decision Outcome");
		decisionOutcomePage
			.withDecisionOutcomeStatus("Approved")
			.withTotalAmountApproved("1,000,000")
			.withDateDecisionMadeDay("12")
			.withDateDecisionMadeMonth("05")
			.withDateDecisionMadeYear("2023")
			.withDecisionTakeEffectDay("14")
			.withDecisionTakeEffectMonth("1")
			.withDecisionTakeEffectYear("2024")
			.withDecisionAuthouriser("Minister")
			.deselectAllBusinessAreas()
			.withBusinessArea("Funding")
			.withBusinessArea("RegionsGroup")
			.saveDecisionOutcome();

		Logger.Log("View Updated Decision Outcome");
		viewDecisionPage
			.hasDecisionOutcomeStatus("Approved")
			.hasTotalAmountApproved("1,000,000")
			.hasMadeDate("12 May 2023")
			.hasEffectiveFromDate("14 January 2024")
			.hasAuthoriser("Minister")
			.hasBusinessArea("Regions Group")
			.hasBusinessArea("Funding");
	});
});
