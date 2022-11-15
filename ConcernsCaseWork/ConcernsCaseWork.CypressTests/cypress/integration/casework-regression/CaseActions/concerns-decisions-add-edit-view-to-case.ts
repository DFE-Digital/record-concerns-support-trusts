import { last } from "cypress/types/lodash/index.js";
import AddToCasePage from "../../../pages/caseActions/addToCasePage.js";
import { ViewDecisionPage } from "../../../pages/caseActions/DecisionPage";

describe("User can add case actions to an existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let stText = "null";
	let condText = "null";
	let reasText = "null";
	let returnedDate = "null";
	let notesText = "null";

	it("Checking that Concerns decision is visible then adding concerns decision case action to a case,  Validation of wrong date when entered", function () {
		cy.addConcernsDecisionsAddToCase();

		AddToCasePage.getDayDateField().click().type("23");
		AddToCasePage.getMonthDateField().click().type("25");
		AddToCasePage.getYearDateField().click().type("2022");
		AddToCasePage.getDecisionButton().click();

		cy.get("#decision-error-list").should(
			"contain.text",
			"23-25-2022 is an invalid date"
		);
	});

	it(" Concern Decision - Checking if View live initial record is visible", function () {
		cy.addConcernsDecisionsAddToCase();

		AddToCasePage.getDayDateField().click().type("12");
		AddToCasePage.getMonthDateField().click().type("05");
		AddToCasePage.getYearDateField().click().type("2022");
		AddToCasePage.getNoticeToImproveBtn().click();
		AddToCasePage.getDecisionButton().click();
		cy.get("#open-case-actions").should(
			"contain.text",
			"Decision: Notice to Improve"
		);
	});

	const decisionPage: ViewDecisionPage = new ViewDecisionPage();

	it(" Concern Decision - Creating a Decision and validating data is visible for this decision", function () {
		
		cy.addConcernsDecisionsAddToCase();
		decisionPage
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
			.withSupportingNotes("These are some supporting notes!");
		AddToCasePage.getDecisionButton().click();
		cy.get("#open-case-actions td")
			.should("contain.text", "Decision: Notice to Improve (NTI)")
			.eq(-3)
			.click();

		decisionPage
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

		decisionPage
			.withCrmEnquiry("777")
			.withRetrospectiveRequest(false)
			.withSubmissionRequired(true)
			.withSubmissionLink("www.google.uk")
			.withDateESFADay("22")
			.withDateESFAMonth("03")
			.withDateESFAYear("2022")
			.withTypeOfDecisionID("QualifiedFloatingCharge")
			.withTotalAmountRequested("£130,000")
			.withSupportingNotes("Testing Supporting Notes");

		decisionPage
			.hasCrmEnquiry("777")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("Yes")
			.hasSubmissionLink("www.google.uk")
			.hasDateESFAReceivedRequest("22-03-2022")
			.hasTotalAmountRequested("£130,000")
			.hasTypeOfDecision("Qualified Floating Charge (QFC)")
			.hasSupportingNotes("Testing Supporting Notes")
			AddToCasePage.getDecisionButton().click();
	});
	it("When Decisions is empty, View Behavior", function () {
		
		cy.addConcernsDecisionsAddToCase();
		AddToCasePage.getDecisionButton().click();
		cy.get("#open-case-actions td")
			.should("contain.text", "Decision: Notice to Improve (NTI)")
			.eq(-3)
			.click();
		decisionPage
			.hasCrmEnquiry("")
			.hasRetrospectiveRequest("")
			.hasSubmissionRequired("")
			.hasSubmissionLink("")
			.hasDateESFAReceivedRequest("")
			.hasTotalAmountRequested("")
			.hasTypeOfDecision("")
			.hasTypeOfDecision("")
			.hasSupportingNotes("")
	});
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
