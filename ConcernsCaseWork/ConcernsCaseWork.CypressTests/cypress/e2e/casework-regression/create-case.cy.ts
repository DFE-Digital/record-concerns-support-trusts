import { Logger } from "cypress/common/logger";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import concernsApi from "cypress/api/concernsApi";
import caseApi from "cypress/api/caseApi";
import createCaseSummary from "cypress/pages/createCase/createCaseSummary";
import validationComponent from "cypress/pages/validationComponent";
import selectCaseTypePage from "cypress/pages/createCase/selectCaseTypePage";
import {
	SourceOfConcernExternal,
	SourceOfConcernInternal,
} from "cypress/constants/selectorConstants";
import selectCaseDivisionPage from "cypress/pages/createCase/selectCaseDivisionPage";

describe("Creating a case", () => {
	const createCasePage = new CreateCasePage();
	const createConcernPage = new CreateConcernPage();
	const addDetailsPage = new AddDetailsPage();
	const addTerritoryPage = new AddTerritoryPage();
	const addConcernDetailsPage = new AddConcernDetailsPage();

	beforeEach(() => {
		cy.login();
	});

	it("Should validate adding a case", () => {
		Logger.Log("Create a case");
		createCasePage
			.createCase()
			.withTrustName("Ashton West End Primary Academy")
			.selectOption();

		Logger.Log("Checking accessibility on finding a trust");
		cy.excuteAccessibilityTests();

		createCasePage.confirmOption();

		createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");

		Logger.Log("Check RegionsGroup is disabled");
        selectCaseDivisionPage
            .hasBeenDisabled("RegionsGroup")

        Logger.Log("You must select a division error");
        selectCaseDivisionPage
            .continue()
            .hasValidationError("Select case division");

        Logger.Log("Checking accessibility on select case division");
        cy.excuteAccessibilityTests();

        Logger.Log("Create a valid case division");
        selectCaseDivisionPage
            .withCaseDivision("SFSO")
            .continue();

		Logger.Log("Check division details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")

		Logger.Log("Check unpopulated territory throws validation error");
		addTerritoryPage.nextStep().hasValidationError("Select SFSO territory");

		Logger.Log("Checking accessibility on territory");
		cy.excuteAccessibilityTests();

		Logger.Log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

		Logger.Log("Check territory details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East");

		Logger.Log("You must select a case error");
		selectCaseTypePage.continue().hasValidationError("Select case type");

		Logger.Log("Checking accessibility on select case type");
		cy.excuteAccessibilityTests();

		Logger.Log("Create a valid concerns case type");
		selectCaseTypePage.withCaseType("Concerns").continue();

		 createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East")

		Logger.Log("Attempt to create an invalid concern");
		createConcernPage.addConcern();

		validationComponent.hasValidationErrorsInOrder([
			"Select concern type",
			"Select concern risk rating",
			"Select means of referral",
		]);

		Logger.Log("Checking accessibility on concern");
		cy.excuteAccessibilityTests();

		Logger.Log("Create a valid concern");
		createConcernPage
			.withConcernType("Deficit")
			.withConcernRating("Red-Amber")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.Log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber");

		createConcernPage.nextStep();

		Logger.Log("Check unpopulated risk to trust throws validation error");
		addDetailsPage.nextStep().hasValidationError("Select risk to trust rating");

		Logger.Log("Checking accessibility on risk to trust");
		cy.excuteAccessibilityTests();

		Logger.Log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red-Plus").nextStep();

		Logger.Log(
			"Check Trust, concern and risk to trust details are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber")
			.hasRiskToTrust("Red Plus");

		Logger.Log(
			"Check Trust, concern, risk to trust details and territory are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber")
			.hasRiskToTrust("Red Plus");

		addConcernDetailsPage.createCase().hasValidationError("Issue is required");

		Logger.Log("Validate unpopulated concern details");
		addConcernDetailsPage
			.withIssueExceedingLimit()
			.withCurrentStatusExceedingLimit()
			.withCaseAimExceedingLimit()
			.withDeEscalationPointExceedingLimit()
			.withNextStepsExceedingLimit()
			.withCaseHistoryExceedingLimit()
			.createCase();

		validationComponent.hasValidationErrorsInOrder([
			"Issue must be 2000 characters or less",
			"Current status must be 4000 characters or less",
			"Case aim must be 1000 characters or less",
			"De-escalation point must be 1000 characters or less",
			"Next steps must be 4000 characters or less",
			"Case notes must be 4300 characters or less",
		]);

		Logger.Log("Checking accessibility on concerns case confirmation");
		cy.excuteAccessibilityTests();

		Logger.Log("Add concern details with valid text limit");
		addConcernDetailsPage
			.withIssue("This is an issue")
			.withCurrentStatus("This is the current status")
			.withCaseAim("This is the case aim")
			.withDeEscalationPoint("This is the de-escalation point")
			.withNextSteps("This is the next steps")
			.withCaseHistory("This is the case history")
			.createCase();

		Logger.Log("Verify case details");
		caseManagementPage
			.hasTrust("Ashton West End Primary Academy")
			.hasRiskToTrust("Red Plus")
			.hasConcerns("Deficit", ["Red", "Amber"])
			.hasTerritory("North and UTC - North East")
			.hasIssue("This is an issue")
			.hasCurrentStatus("This is the current status")
			.hasCaseAim("This is the case aim")
			.hasDeEscalationPoint("This is the de-escalation point")
			.hasNextSteps("This is the next steps")
			.hasCaseHistory("This is the case history");

		Logger.Log("Checking accessibility on case management");
		cy.excuteAccessibilityTests();

		Logger.Log("Verify the means of referral is set");
		caseManagementPage.getCaseIDText().then((caseId) => {
			concernsApi.get(parseInt(caseId)).then((response) => {
				expect(response[0].meansOfReferralId).to.eq(2);
			});
		});
		Logger.Log("Verify Trust Companise House Number is set on the API");
		caseManagementPage.getCaseIDText().then((caseId) => {
			caseApi.get(parseInt(caseId)).then((response) => {
				expect(response.trustCompaniesHouseNumber).to.eq("09388819");
			});
		});
	});

	it("Should create a case with only required fields", () => {
		Logger.Log("Create a case");
		createCasePage
			.createCase()
			.withTrustName("Ashton West End Primary Academy")
			.selectOption()
			.confirmOption();

		createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");

        Logger.Log("Create a valid case division");
        selectCaseDivisionPage
            .withCaseDivision("SFSO")
            .continue();

		Logger.Log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

        Logger.Log("Create a valid concerns case type");
        selectCaseTypePage
            .withCaseType("Concerns")
            .continue();

		createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");

		Logger.Log("Create a valid concern");
		createConcernPage
			.withConcernType("Force majeure")
			.withConcernRating("Amber-Green")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.Log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East")
			.hasConcernRiskRating("Amber Green")
			.hasConcernType("Force majeure");

		createConcernPage.nextStep();

		Logger.Log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red").nextStep();

		Logger.Log(
			"Check Trust, concern and risk to trust details are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East")
			.hasConcernType("Force majeure")
			.hasConcernRiskRating("Amber Green")
			.hasRiskToTrust("Red");

		Logger.Log(
			"Check Trust, concern, risk to trust details and territory are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO")
			.hasManagedBy("North and UTC - North East")
			.hasConcernType("Force majeure")
			.hasConcernRiskRating("Amber Green")
			.hasRiskToTrust("Red")

		Logger.Log("Add concern details with valid text limit");
		addConcernDetailsPage.withIssue("This is an issue").createCase();

		Logger.Log("Verify case details");
		caseManagementPage
			.hasTrust("Ashton West End Primary Academy")
			.hasRiskToTrust("Red")
			.hasConcerns("Force majeure", ["Amber", "Green"])
			.hasTerritory("North and UTC - North East")
			.hasIssue("This is an issue")
			.hasEmptyCurrentStatus()
			.hasEmptyCaseAim()
			.hasEmptyDeEscalationPoint()
			.hasEmptyNextSteps()
			.hasEmptyCaseHistory();
	});

	it("Should display an error if no trust is selected", () => {
		createCasePage
			.createCase()
			.withTrustName("A")
			.confirmOption()
			.hasValidationError("A trust must be selected");
	});

	it("Should display a warning if too many results", () => {
		createCasePage
			.createCase()
			.withTrustName("school")
			.shouldNotHaveVisibleLoader()
			.hasTooManyResultsWarning(
				"There are a large number of search results. Try a more specific search term."
			);
	});

	it("Should create additional concerns", () => {
		Logger.Log("Create a case");
		createCasePage
			.createCase()
			.withTrustName("Ashton West End Primary Academy")
			.selectOption()
			.confirmOption();

		Logger.Log("Create a valid case division");
		selectCaseDivisionPage
			.withCaseDivision("SFSO")
			.continue();

		Logger.Log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

		Logger.Log("Create a valid concerns case type");
		selectCaseTypePage.withCaseType("Concerns").continue();

		Logger.Log("Create a valid concern");
		createConcernPage
			.withConcernType("Suspected fraud")
			.withConcernRating("Red-Plus")
			.withMeansOfReferral(SourceOfConcernInternal)
			.addConcern();

		Logger.Log("Adding another concern during case creation");
		createConcernPage
			.addAnotherConcern()
			.withConcernType("Financial compliance")
			.withConcernRating("Amber-Green")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern()
			.nextStep();

		Logger.Log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red-Plus").nextStep();

		Logger.Log("Add concern details with valid text limit");
		addConcernDetailsPage.withIssue("This is an issue").createCase();

		Logger.Log("Add another concern after case creation");
		caseManagementPage.addAnotherConcern();

		Logger.Log("Attempt to create an invalid concern");
		createConcernPage
			.addConcern()
			.hasValidationError("Select concern type")
			.hasValidationError("Select concern risk rating")
			.hasValidationError("Select means of referral");

		createConcernPage
			.withConcernType("Irregularity")
			.withConcernRating("Red-Amber")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		caseManagementPage
			.hasConcerns("Suspected fraud", ["Red Plus"])
			.hasConcerns("Financial compliance", ["Amber", "Green"])
			.hasConcerns("Irregularity", ["Red", "Amber"]);
	});
});
