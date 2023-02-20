import { Logger } from "../../../common/logger";
import { EditDecisionPage } from "../../../pages/caseActions/decision/editDecisionPage";
import { ViewDecisionPage } from "../../../pages/caseActions/decision/viewDecisionPage";
import { CloseDecisionPage } from "../../../pages/caseActions/decision/closeDecisionPage";
import { DecisionOutcomePage } from "../../../pages/caseActions/decision/decisionOutcomePage";
import "cypress-axe";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";

describe("User can add case actions to an existing case", () => {
	const viewDecisionPage = new ViewDecisionPage();
	const editDecisionPage = new EditDecisionPage();
	const closeDecisionPage = new CloseDecisionPage();
	const decisionOutcomePage = new DecisionOutcomePage();

	let now;
	
	beforeEach(() => {
		cy.login();
		now = new Date();
	});

	it("Concern Decision - Creating a Decision and validating data is visible for this decision", function () {
		cy.addConcernsDecisionsAddToCase();

		Logger.Log("Checking an invalid date");
		editDecisionPage
			.withDateESFADay("23")
			.withDateESFAMonth("25")
			.withDateESFAYear("2022")
			.save()
			.hasValidationError(
				"Date ESFA received request: 23-25-2022 is an invalid date"
			);

		Logger.Log("Checking an incomplete date with notes exceeded");
		editDecisionPage
			.withDateESFADay("23")
			.withDateESFAMonth("12")
			.withDateESFAYear("")
			.withSupportingNotesExceedingLimit()
			.save()
			.hasValidationError(
				"Date ESFA received request: Please enter a complete date DD MM YYYY"
			)
			.hasValidationError("Notes must be 2000 characters or less");

		Logger.Log("Creating Decision");
		editDecisionPage
			.withCrmEnquiry("444")
			.withRetrospectiveRequest(false)
			.withSubmissionRequired(true)
			.withSubmissionLink("www.gov.uk")
			.withDateESFADay("21")
			.withDateESFAMonth("04")
			.withDateESFAYear("2022")
			.withTypeOfDecisionID("NoticeToImprove")
			.withTypeOfDecisionID("Section128")
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
				row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

		Logger.Log("Viewing Decision");
		viewDecisionPage
			.hasDateDecisionOpened(toDisplayDate(now))
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
			.withCrmEnquiry("777")
			.withRetrospectiveRequest(false)
			.withSubmissionRequired(true)
			.withSubmissionLink("www.google.uk")
			.withDateESFADay("22")
			.withDateESFAMonth("03")
			.withDateESFAYear("2022")
			.withTypeOfDecisionID("QualifiedFloatingCharge")
			.withTotalAmountRequested("£130,000")
			.withSupportingNotes("Testing Supporting Notes")
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

	it("Concern Decision - Creating a case, then creating a Decision, validating data is visible for this decision then Close the decision", function () {
		cy.addConcernsDecisionsAddToCase();

		Logger.Log("Adding note on the decision that will be closing ");
		editDecisionPage
			.withCrmEnquiry("444")
			.withRetrospectiveRequest(false)
			.withSubmissionRequired(true)
			.withSubmissionLink("www.gov.uk")
			.withDateESFADay("21")
			.withDateESFAMonth("04")
			.withDateESFAYear("2022")
			.withTypeOfDecisionID("NoticeToImprove")
			.withTypeOfDecisionID("Section128")
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
			.withBusinessArea("ProviderMarketOversight")
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

		Logger.Log("Validating notes can not exceed limits");
		closeDecisionPage
			.withSupportingNotesExceedingLimit()
			.closeDecision()
			.hasValidationError("Supporting Notes must be 2000 characters or less");

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
			.hasDateDecisionOpened(toDisplayDate(now))
			.hasDateDecisionClosed(toDisplayDate(now))
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
			.hasBusinessArea("Provider Market Oversight")
			.hasDecisionOutcomeStatus("Approved with conditions")
			.hasMadeDate("24 November 2022")
			.hasEffectiveFromDate("11 December 2023")
			.hasTotalAmountApproved("£50,000")
			.hasAuthoriser("Deputy Director")
			.cannotCreateAnotherDecisionOutcome()
			.cannotCloseDecision()
			.cannotEditDecision()
			.cannotEditDecisionOutcome();
	});

	it("When Decisions is empty, View Behavior", function () {
		Logger.Log("Creating Empty Decision");
		cy.addConcernsDecisionsAddToCase();

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

		Logger.Log("Viewing Empty Decision");
		viewDecisionPage
			.hasDateDecisionOpened(toDisplayDate(now))
			.hasCrmEnquiry("Empty")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("No")
			.hasSubmissionLink("Empty")
			.hasDateESFAReceivedRequest("Empty")
			.hasTotalAmountRequested("£0.00")
			.hasTypeOfDecision("Empty")
			.hasSupportingNotes("Empty");
	});

	it("Edit a decision outcome ", () => {
		Logger.Log("Creating Empty Decision");
		cy.addConcernsDecisionsAddToCase();

		editDecisionPage
			.save();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		viewDecisionPage
			.createDecisionOutcome();

		Logger.Log("Create Decision Outcome");
		decisionOutcomePage
			.withDecisionOutcomeStatus("Withdrawn")
			.withTotalAmountApproved("1,000")
			.withDateDecisionMadeDay("3")
			.withDateDecisionMadeMonth("5")
			.withDateDecisionMadeYear("2022")
			.withDecisionTakeEffectDay("6")
			.withDecisionTakeEffectMonth("8")
			.withDecisionTakeEffectYear("2022")
			.withDecisionAuthouriser("DeputyDirector")
			.withBusinessArea("BusinessPartner")
			.withBusinessArea("Capital")
			.withBusinessArea("ProviderMarketOversight")
			.saveDecisionOutcome();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		viewDecisionPage
			.editDecisionOutcome();

		Logger.Log("Verify Existing Values");
		decisionOutcomePage
			.hasDecisionOutcomeStatus("Withdrawn")
			.hasTotalAmountApproved("1,000")
			.hasDecisionMadeDay("3")
			.hasDecisionMadeMonth("5")
			.hasDecisionMadeYear("2022")
			.hasDateEffectiveFromDay("6")
			.hasDateEffectiveFromMonth("8")
			.hasDateEffectiveFromYear("2022")
			.hasDecisionAuthouriser("DeputyDirector")
			.hasBusinessArea("BusinessPartner")
			.hasBusinessArea("Capital")
			.hasBusinessArea("ProviderMarketOversight");


		Logger.Log("Edit Decision Outcome");
		decisionOutcomePage
			.withDecisionOutcomeStatus("Approved")
			.withTotalAmountApproved("1,000,000")
			.withDateDecisionMadeDay("12")
			.withDateDecisionMadeMonth("11")
			.withDateDecisionMadeYear("2023")
			.withDecisionTakeEffectDay("14")
			.withDecisionTakeEffectMonth("1")
			.withDecisionTakeEffectYear("2024")
			.withDecisionAuthouriser("Minister")
			.saveDecisionOutcome();


		Logger.Log("View Updated Decision Outcome");
		viewDecisionPage
			.hasDecisionOutcomeStatus("Approved")
			.hasTotalAmountApproved("1,000,000")
			.hasMadeDate("12 November 2023")
			.hasEffectiveFromDate("14 January 2024")
			.hasAuthoriser("Minister")
			.hasBusinessArea("Business Partner")
			.hasBusinessArea("Capital")
			.hasBusinessArea("Provider Market Oversight (PMO)");

	})

	it("Create a decision outcome, checking validation and view it was created correctly", () => {
		Logger.Log("Creating Empty Decision");
		cy.addConcernsDecisionsAddToCase();

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
			.hasValidationError("Date decision made: 24-13-2022 is an invalid date")
			.hasValidationError("Date decision effective: 12-22-2023 is an invalid date")

		Logger.Log("Checking an incomplete dates");
		decisionOutcomePage
			.withDateDecisionMadeDay("24")
			.withDateDecisionMadeMonth("12")
			.withDateDecisionMadeYear("")
			.withDecisionTakeEffectDay("12")
			.withDecisionTakeEffectMonth("06")
			.withDecisionTakeEffectYear("")
			.saveDecisionOutcome()
			.hasValidationError("Date decision made: Please enter a complete date DD MM YYYY")
			.hasValidationError("Date decision effective: Please enter a complete date DD MM YYYY");

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
			.withBusinessArea("ProviderMarketOversight")
			.saveDecisionOutcome();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		Logger.Log("View decision outcome")

		viewDecisionPage
			.hasBusinessArea("Business Partner")
			.hasBusinessArea("Capital")
			.hasBusinessArea("Provider Market Oversight")
			.hasDecisionOutcomeStatus("Approved with conditions")
			.hasMadeDate("24 November 2022")
			.hasEffectiveFromDate("11 December 2023")
			.hasTotalAmountApproved("£50,000")
			.hasAuthoriser("Deputy Director")
			.cannotCreateAnotherDecisionOutcome();
	});

	it("Create a decision outcome with only status, should set status but all other labels should be empty", () => {
		Logger.Log("Creating Empty Decision");
		cy.addConcernsDecisionsAddToCase();

		editDecisionPage
			.save();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		viewDecisionPage
			.createDecisionOutcome();

		Logger.Log("Create Decision Outcome with only status");
		decisionOutcomePage
			.withDecisionOutcomeStatus("Withdrawn")
			.saveDecisionOutcome();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then(row =>
			{
				row.select();
			});

		viewDecisionPage
			.hasBusinessArea("Empty")
			.hasDecisionOutcomeStatus("Withdrawn")
			.hasMadeDate("Empty")
			.hasEffectiveFromDate("Empty")
			.hasTotalAmountApproved("£0.00")
			.hasAuthoriser("Empty");
	});
});
