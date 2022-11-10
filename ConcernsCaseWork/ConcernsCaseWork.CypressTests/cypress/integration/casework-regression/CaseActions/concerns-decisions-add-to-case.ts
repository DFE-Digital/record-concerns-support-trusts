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

	it.only(" Concern Decision - Creating a Decision and validating data is visible ", function () {
		cy.addConcernsDecisionsAddToCase();

		AddToCasePage.getDayDateField().click().type("12");
		AddToCasePage.getMonthDateField().click().type("05");
		AddToCasePage.getYearDateField().click().type("2022");
		AddToCasePage.getNoticeToImproveBtn().click();
		AddToCasePage.getDecisionButton().click();
		cy.get('#open-case-actions td').should(
			"contain.text",
			"Decision: Notice to Improve (NTI)"
		).eq(0).click();
		decisionPage
			.hasCrmEnquiry("123456")
			.hasRetrospectiveRequest("No")
			.hasSubmissionRequired("No");



	});
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
