import { last } from "cypress/types/lodash/index.js";
import AddToCasePage from "../../../pages/caseActions/addToCasePage.js";
import { ViewDecisionPage } from "../../../pages/caseActions/viewDecisionPage";

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

		cy.get('#decision-error-list').should(
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
		cy.get('#open-case-actions').should(
			"contain.text",
			"Decision: Notice to Improve"
		);


	});

	const decisionPage: ViewDecisionPage = new ViewDecisionPage();

	it.only(" Concern Decision - Creating a Decision and validating data is visible for this decision", function () {
		cy.addConcernsDecisionsAddToCase();
		decisionPage
			.withCrmEnquiry("444")
			.withRetrospectiveRequest("No")
			.withSubmissionRequired("No")
			.withSubmissionLink("www.gov.uk")
			.withDateESFAReceivedRequest("20-04-2022")
			.withTotalAmountRequested("£140,000")
			.withTypeOfDecision("Repayable support")
			.withSupportingNotes("These are some supporting notes!")
			.withActionEdit("Edit");

		AddToCasePage.getDayDateField().click().type("12");
		AddToCasePage.getMonthDateField().click().type("05");
		AddToCasePage.getYearDateField().click().type("2022");
		AddToCasePage.getNoticeToImproveBtn().click();
		AddToCasePage.getDecisionButton().click();
		cy.get('#open-case-actions td').should(
			"contain.text",
			"Decision: Notice to Improve (NTI)"
		).eq(-3).click();
		decisionPage
			.hasCrmEnquiry("444")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("No")
			.hasSubmissionLink("www.gov.uk")
			.hasDateESFAReceivedRequest("20-04-2022")
			.hasTotalAmountRequested("£140,000")
			.hasTypeOfDecision("Repayable support")
			.hasSupportingNotes("These are some supporting notes!")
			.hasActionEdit("Edit");
	});
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
