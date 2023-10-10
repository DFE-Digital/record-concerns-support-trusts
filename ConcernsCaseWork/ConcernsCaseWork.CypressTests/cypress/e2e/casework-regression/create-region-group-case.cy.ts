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



	it("Should create a case with region group", () => {
		Logger.Log("Create a case");
		createCasePage
			.createCase()
			.withTrustName("Ashton West End Primary Academy")
			.selectOption()
			.confirmOption();

		createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");

        Logger.Log("Create a valid case division");
        selectCaseDivisionPage
            .withCaseDivision("RegionsGroup")
            .continue();

		Logger.Log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

      
		createCaseSummary
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .hasManagedBy("Regions Group", "North and UTC - North East");

		Logger.Log("Create a valid concern");
		createConcernPage
			.withConcernType("Force majeure")
			.withConcernRating("Amber-Green")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.Log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "North and UTC - North East")
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
			.hasManagedBy("Regions Group", "North and UTC - North East")
			.hasConcernType("Force majeure")
			.hasConcernRiskRating("Amber Green")
			.hasRiskToTrust("Red");

		Logger.Log(
			"Check Trust, concern, risk to trust details and territory are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "North and UTC - North East")
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
			.hasManagedBy("Regions Group", "North and UTC - North East")
			.hasIssue("This is an issue")
			.hasEmptyCurrentStatus()
			.hasEmptyCaseAim()
			.hasEmptyDeEscalationPoint()
			.hasEmptyNextSteps()
			.hasEmptyCaseHistory();
	});

});


