import { Logger } from "../../../common/logger";
import { EditDecisionPage } from "../../../pages/caseActions/decision/editDecisionPage";
import { ViewDecisionPage } from "../../../pages/caseActions/decision/viewDecisionPage";

describe("User can add case actions to an existing case", () => {
	const viewDecisionPage = new ViewDecisionPage();
	const editDecisionPage = new EditDecisionPage();

	beforeEach(() => {
		cy.login();
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

	it(" Concern Decision - Creating a Decision and validating data is visible for this decision", function () {
		cy.addConcernsDecisionsAddToCase();

		Logger.Log("Checking an invalid date");
		editDecisionPage
			.withDateESFADay("23")
			.withDateESFAMonth("25")
			.withDateESFAYear("2022")
			.saveDecision()
			.hasValidationError("23-25-2022 is an invalid date");

		Logger.Log("Checking an incomplete date with notes exceeded");
		editDecisionPage
			.withDateESFADay("23")
			.withDateESFAMonth("12")
			.withDateESFAYear("")
			.withSupportingNotesExceedingLimit()
			.saveDecision()
			.hasValidationError("Please enter a complete date DD MM YYYY")
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
			.saveDecision();

		Logger.Log("Selecting Decision from open actions");
		cy.get("#open-case-actions td")
			.should("contain.text", "Decision: Multiple Decision Types")
			.eq(-3)
			.click();

		Logger.Log("Viewing Decision");
		viewDecisionPage
			.hasCrmEnquiry("444")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("Yes")
			.hasSubmissionLink("www.gov.uk")
			.hasDateESFAReceivedRequest("21-04-2022")
			.hasTotalAmountRequested("£140,000")
			.hasTypeOfDecision("Notice to Improve (NTI)")
			.hasTypeOfDecision("Section 128 (S128)")
			.hasSupportingNotes("These are some supporting notes!")
			.hasActionEdit()
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
			.saveDecision();

		Logger.Log("Viewing Edited Decision");
		viewDecisionPage
			.hasCrmEnquiry("777")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("Yes")
			.hasSubmissionLink("www.google.uk")
			.hasDateESFAReceivedRequest("22-03-2022")
			.hasTotalAmountRequested("£130,000")
			.hasTypeOfDecision("Qualified Floating Charge (QFC)")
			.hasSupportingNotes("Testing Supporting Notes");
	});

	it("When Decisions is empty, View Behavior", function () {
		Logger.Log("Creating Empty Decision");
		cy.addConcernsDecisionsAddToCase();

		editDecisionPage.saveDecision();

		cy.get("#open-case-actions td")
			.should("contain.text", "Decision: No Decision Types")
			.eq(-3)
			.click();

		Logger.Log("Viewing Empty Decision");
		viewDecisionPage
			.hasCrmEnquiry("Empty")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("No")
			.hasSubmissionLink("Empty")
			.hasDateESFAReceivedRequest("Empty")
			.hasTotalAmountRequested("£0.00")
			.hasTypeOfDecision("Empty")
			.hasSupportingNotes("Empty");
	});
});
