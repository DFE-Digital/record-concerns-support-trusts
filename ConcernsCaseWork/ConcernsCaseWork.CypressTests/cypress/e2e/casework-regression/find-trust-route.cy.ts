import trustOverviewPage from "cypress/pages/trustOverviewPage";
import { Logger } from "../../common/logger";
import createCaseSummary from "cypress/pages/createCase/createCaseSummary";
import selectCaseTypePage from "cypress/pages/createCase/selectCaseTypePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";

describe("User interactions via Find Trust route", () => {

    const createConcernPage = new CreateConcernPage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    const addDetailsPage = new AddDetailsPage();

	beforeEach(() => {
		cy.login();
	});

	const trustName = "Accrington St Christopher's Church Of England High School";

	it("Should display details on the selected trust and create a case against the trust", () => {
		Logger.Log("User clicks on Create Case and should see Search Trusts");
		cy.get('[href="/trust"]').click();
		cy.get("#search").should("be.visible");
	
		Logger.Log("It should display an error when the user does not enter a trust");
		cy.get("#search").type(trustName.substring(0, 1) + "{enter}");
		cy.getById("errorSummary").should("contain.text", "A trust must be selected");
	
		Logger.Log("Getting too many search results should result in a warning");
		const warningMessage = "There are a large number of search results. Try a more specific search term.";
		cy.get("#search").type("sch");
		cy.getById("tooManyResultsWarning").should(
			"contain.text",
			warningMessage
		);
	
		Logger.Log("Search for a valid trust should find multiple results");
		cy.get("#search").clear().type("Manchester");
		cy.get("#search__listbox").children().should("have.length.gt", 1);
	
		Logger.Log("Selecting the trust should populate it in the search");
		cy.get("#search").clear().type(trustName);
		cy.get("#search__option--0").click();
		cy.get("#search").should(
			"have.value",
			`${trustName}, 10059220, 07728029 (Accrington)`
		);
	
		Logger.Log("Clicking continue should populate the trust page");
		cy.getById("continue").click();
		cy.url().should("include", "/trust");
	
		Logger.Log("Trust page should have title of the academy");
		cy.get(".govuk-heading-l").should(
			"contain.text",
			trustName
		);
		
		Logger.Log("Ensure that the trust information is populated");
		trustOverviewPage
			.trustTypeIsNotEmpty()
			.trustAddressIsNotEmpty()
			.trustAcademiesIsNotEmpty()
			.trustPupilCapacityIsNotEmpty()
			.trustPupilNumbersIsNotEmpty()
			.trustGroupIdIsNotEmpty()
			.trustUKPRNIsNotEmpty()
			.trustCompanyHouseNumberIsNotEmpty();

		Logger.Log("Checking accessibility on risk to trust");
		cy.excuteAccessibilityTests();

		Logger.Log("Create a case against the trust");
		trustOverviewPage.createCase();

		createCaseSummary
			.hasTrustSummaryDetails(trustName);

		selectCaseTypePage
			.withCaseType("Concerns")
			.continue();

		Logger.Log("Create a valid concern");
		createConcernPage
			.withConcernType("Financial")
			.withSubConcernType("Financial: Deficit")
			.withConcernRating("Red-Amber")
			.withMeansOfReferral("External")
			.addConcern();

		Logger.Log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasConcernType("Financial: Deficit")
			.hasConcernRiskRating("Red Amber");

		createConcernPage
			.nextStep();
		
		Logger.Log("Populate risk to trust");
		addDetailsPage
			.withRiskToTrust("Red-Plus")
			.nextStep();

		Logger.Log("Check Trust, concern and risk to trust details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasConcernType("Financial: Deficit")
			.hasConcernRiskRating("Red Amber")
			.hasRiskToTrust("Red Plus");

		Logger.Log("Populate territory");
		addTerritoryPage
			.withTerritory("North and UTC - North East")
			.nextStep();

		Logger.Log("Check Trust, concern, risk to trust details and territory are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasConcernType("Financial: Deficit")
			.hasConcernRiskRating("Red Amber")
			.hasRiskToTrust("Red Plus")
			.hasTerritory("North and UTC - North East");

		Logger.Log("Add concern details with valid text limit");
		addConcernDetailsPage
			.withIssue("This is an issue")
			.createCase();

		Logger.Log("Verify case details");
		caseManagementPage
			.hasTrust(trustName)
			.hasRiskToTrust("Red Plus")
			.hasConcerns("Financial: Deficit", ["Red", "Amber"])
			.hasTerritory("North and UTC - North East")
			.hasIssue("This is an issue");
	});
});
	
