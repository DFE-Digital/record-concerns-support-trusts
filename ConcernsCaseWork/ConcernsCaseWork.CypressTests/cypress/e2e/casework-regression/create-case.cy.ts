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
import { CaseBuilder } from "cypress/api/caseBuilder";

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

		Logger.log("Checking accessibility on home page");
        cy.excuteAccessibilityTests();	

		Logger.log("Create a case");
		createCasePage
			.createCase()
			.withTrustName("Ashton West End Primary Academy")
			.selectOption();

		Logger.log("Checking accessibility on finding a trust");
		cy.excuteAccessibilityTests();

		createCasePage.confirmOption();

		createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");

        Logger.log("You must select a division error");
        selectCaseDivisionPage
            .continue()
            .hasValidationError("Select case division");

        Logger.log("Checking accessibility on select case division");
        cy.excuteAccessibilityTests();

        Logger.log("Create a valid case division");
        selectCaseDivisionPage
            .withCaseDivision("SFSO")
            .continue();

		Logger.log("Check division details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "");

		Logger.log("Check unpopulated territory throws validation error");
		addTerritoryPage.nextStep().hasValidationError("Select SFSO territory");

		Logger.log("Checking accessibility on territory");
		cy.excuteAccessibilityTests();

		Logger.log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

		Logger.log("Check territory details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")

		Logger.log("You must select a case error");
		selectCaseTypePage.continue().hasValidationError("Select case type");

		Logger.log("Checking accessibility on select case type");
		cy.excuteAccessibilityTests();

		Logger.log("Create a valid concerns case type");
		selectCaseTypePage.withCaseType("Concerns").continue();

		 createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East");

		Logger.log("Attempt to create an invalid concern");
		createConcernPage.addConcern();

		validationComponent.hasValidationErrorsInOrder([
			"Select concern type",
			"Select concern risk rating",
			"Select means of referral",
		]);

		Logger.log("Checking accessibility on concern");
		cy.excuteAccessibilityTests();

		Logger.log("Check has SFSO specific means of referral hint text")
		createCaseSummary
			.hasHintText("For example, management letter, external review of governance, ESFA activity or other departmental activity.")
			.hasHintText("For example, whistleblowing, self-reported, SCCU, CIU casework, regional director (RD), Ofsted or other government bodies.");

		Logger.log("Create a valid concern");
		createConcernPage
			.withConcernType("Deficit")
			.withConcernRating("Red-Amber")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber");

		createConcernPage.nextStep();

		Logger.log("Check unpopulated risk to trust throws validation error");
		addDetailsPage.nextStep().hasValidationError("Select risk to trust rating");

		Logger.log("Checking accessibility on risk to trust");
		cy.excuteAccessibilityTests();

		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber");

		Logger.log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red Plus").nextStep();

		Logger.log(
			"Check Trust, concern and risk to trust details are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber")
			.hasRiskToTrust("Red Plus");

		Logger.log(
			"Check Trust, concern, risk to trust details and territory are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber")
			.hasRiskToTrust("Red Plus");

		addConcernDetailsPage.createCase().hasValidationError("Issue is required");

		Logger.log("Validate unpopulated concern details");
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

		Logger.log("Checking accessibility on concerns case confirmation");
		cy.excuteAccessibilityTests();

		Logger.log("Add concern details with valid text limit");
		addConcernDetailsPage
			.withIssue("This is an issue")
			.withCurrentStatus("This is the current status")
			.withCaseAim("This is the case aim")
			.withDeEscalationPoint("This is the de-escalation point")
			.withNextSteps("This is the next steps")
			.withCaseHistory("This is the case history")
			.createCase();

		Logger.log("Verify case details");
		caseManagementPage
			.hasTrust("Ashton West End Primary Academy")
			.hasRiskToTrust("Red Plus")
			.hasConcerns("Deficit", ["Red", "Amber"])
			.hasNumberOfConcerns(1)
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasIssue("This is an issue")
			.hasCurrentStatus("This is the current status")
			.hasCaseAim("This is the case aim")
			.hasDeEscalationPoint("This is the de-escalation point")
			.hasNextSteps("This is the next steps")
			.hasCaseHistory("This is the case history");

		Logger.log("Checking accessibility on case management");
		cy.excuteAccessibilityTests();

		Logger.log("Verify the means of referral is set");
		caseManagementPage.getCaseIDText().then((caseId) => {
			concernsApi.get(parseInt(caseId)).then((response) => {
				expect(response[0].meansOfReferralId).to.eq(2);
			});
		});
		Logger.log("Verify Trust Companise House Number is set on the API");
		caseManagementPage.getCaseIDText().then((caseId) => {
			caseApi.get(parseInt(caseId)).then((response) => {
				expect(response.trustCompaniesHouseNumber).to.eq("09388819");
			});
		});
	});

	it("Should create a case with only required fields", () => {
		Logger.log("Create a case");
		createCasePage
			.createCase()
			.withTrustName("Ashton West End Primary Academy")
			.selectOption()
			.confirmOption();

		createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");

        Logger.log("Create a valid case division");
        selectCaseDivisionPage
            .withCaseDivision("SFSO")
            .continue();

		Logger.log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

        Logger.log("Create a valid concerns case type");
        selectCaseTypePage
            .withCaseType("Concerns")
            .continue();

		createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");

		Logger.log("Create a valid concern");
		createConcernPage
			.withConcernType("Force majeure")
			.withConcernRating("Amber-Green")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernRiskRating("Amber Green")
			.hasConcernType("Force majeure");

		createConcernPage.nextStep();

		Logger.log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red").nextStep();

		Logger.log(
			"Check Trust, concern and risk to trust details are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Force majeure")
			.hasConcernRiskRating("Amber Green")
			.hasRiskToTrust("Red");

		Logger.log(
			"Check Trust, concern, risk to trust details and territory are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Force majeure")
			.hasConcernRiskRating("Amber Green")
			.hasRiskToTrust("Red")

		Logger.log("Add concern details with valid text limit");
		addConcernDetailsPage.withIssue("This is an issue").createCase();

		Logger.log("Verify case details");
		caseManagementPage
			.hasTrust("Ashton West End Primary Academy")
			.hasRiskToTrust("Red")
			.hasConcerns("Force majeure", ["Amber", "Green"])
			.hasManagedBy("SFSO", "North and UTC - North East")
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
		Logger.log("Create a case");
		createCasePage
			.createCase()
			.withTrustName("Ashton West End Primary Academy")
			.selectOption()
			.confirmOption();

		Logger.log("Create a valid case division");
		selectCaseDivisionPage
			.withCaseDivision("SFSO")
			.continue();

		Logger.log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

		Logger.log("Create a valid concerns case type");
		selectCaseTypePage.withCaseType("Concerns").continue();

		Logger.log("Create a valid concern");
		createConcernPage
			.withConcernType("Suspected fraud")
			.withConcernRating("Red Plus")
			.withMeansOfReferral(SourceOfConcernInternal)
			.addConcern();

		Logger.log("Adding another concern during case creation");
		createConcernPage
			.addAnotherConcern()
			.withConcernType("Financial compliance")
			.withConcernRating("Amber-Green")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern()
			.nextStep();

		Logger.log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red Plus").nextStep();

		Logger.log("Add concern details with valid text limit");
		addConcernDetailsPage.withIssue("This is an issue").createCase();

		Logger.log("Add another concern after case creation");
		caseManagementPage.addAnotherConcern();

		Logger.log("Attempt to create an invalid concern");
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
			.hasConcerns("Irregularity", ["Red", "Amber"])
			.hasNumberOfConcerns(3);
	});

	describe("When we create a case with the minimum data", () =>
	{
		let expectedCaseId: number;

		beforeEach(() =>
		{
			const request = CaseBuilder.buildOpenCaseMinimumCriteria();

			cy.createNonConcernsCase(request)
			.then(response =>
			{
				expectedCaseId = response.urn;
			})
		});

		it("Should render on case management", () =>
		{
			caseManagementPage.getCaseIDText()
			.then(actualCaseId =>
			{
				expect(expectedCaseId.toString()).to.equal(actualCaseId);
			});
		});
	});
});
