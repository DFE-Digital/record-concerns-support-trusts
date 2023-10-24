import { Logger } from "cypress/common/logger";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import createCaseSummary from "cypress/pages/createCase/createCaseSummary";
import {
	SourceOfConcernExternal,
} from "cypress/constants/selectorConstants";
import selectCaseDivisionPage from "cypress/pages/createCase/selectCaseDivisionPage";
import addRegionPage from "cypress/pages/createCase/addRegionPage";
import validationComponent from "cypress/pages/validationComponent";
import editRegionPage from "cypress/pages/createCase/editRegionPage";
import editConcernPage from "cypress/pages/editConcernPage";
import homePage from "cypress/pages/homePage";
import closedCasePage from "cypress/pages/closedCasePage";
import { ViewClosedCasePage } from "cypress/pages/createCase/viewClosedCasePage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { EditSrmaPage } from "cypress/pages/caseActions/srma/editSrmaPage";
import { ViewSrmaPage } from "cypress/pages/caseActions/srma/viewSrmaPage";
import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";


describe("Creating a case", () => {
	const createCasePage = new CreateCasePage();
	const createConcernPage = new CreateConcernPage();
	const addDetailsPage = new AddDetailsPage();
	const addConcernDetailsPage = new AddConcernDetailsPage();
	const viewClosedCasePage = new ViewClosedCasePage();
	const editSrmaPage = new EditSrmaPage();
	const viewSrmaPage = new ViewSrmaPage();
	let now: Date;

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

		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "");

		Logger.Log("Check validation error if region is not selected");
		addRegionPage.nextStep();
		validationComponent.hasValidationError("Select region");

		cy.excuteAccessibilityTests();

		Logger.Log("Select valid region");
		addRegionPage
			.withRegion("London")
			.nextStep();

		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London");

		Logger.Log("Create a valid concern");
		createConcernPage
			.withConcernType("Governance")
			.withConcernRating("Amber-Green")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.Log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London")
			.hasConcernRiskRating("Amber Green")
			.hasConcernType("Governance");

		createConcernPage.nextStep();

		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London")
			.hasConcernRiskRating("Amber Green")
			.hasConcernType("Governance");

		Logger.Log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red").nextStep();

		Logger.Log(
			"Check Trust, concern and risk to trust details are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London")
			.hasConcernType("Governance")
			.hasConcernRiskRating("Amber Green")
			.hasRiskToTrust("Red");

		Logger.Log("Add concern details with valid text limit");
		addConcernDetailsPage.withIssue("This is an issue").createCase();

		Logger.Log("Verify case details");
		caseManagementPage
			.hasTrust("Ashton West End Primary Academy")
			.hasRiskToTrust("Red")
			.hasConcerns("Governance", ["Amber", "Green"])
			.hasManagedBy("Regions Group", "London")
			.hasIssue("This is an issue");

		Logger.Log("Editing the existing region");
		caseManagementPage.editManagedBy();

		editRegionPage
			.hasRegion("London")
			.withRegion("South West")
			.apply();

		caseManagementPage.hasManagedBy("Regions Group", "South West");

		Logger.Log("Add another concern after case creation");
		caseManagementPage.addAnotherConcern();

		createConcernPage
			.withConcernType("Safeguarding")
			.withConcernRating("Red-Amber")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		caseManagementPage
			.hasConcerns("Governance", ["Amber", "Green"])
			.hasConcerns("Safeguarding", ["Red", "Amber"]);

		Logger.Log("Close down the concerns");
		caseManagementPage
			.getEditConcern().first().click();

		editConcernPage.closeConcern().confirmCloseConcern();

		caseManagementPage
			.getEditConcern().first().click();

		editConcernPage.closeConcern().confirmCloseConcern();

		caseManagementPage.getCaseIDText()
			.then((caseId => {
				caseManagementPage.getCloseCaseBtn().click();
				caseManagementPage.withRationaleForClosure("Closing").getCloseCaseBtn().click();

				Logger.Log("Viewing case is closed");
				homePage.getClosedCasesBtn().click();

				Logger.Log("Checking accessibility on closed case");
				cy.excuteAccessibilityTests();

				closedCasePage.getClosedCase(caseId).click();

				viewClosedCasePage.hasManagedBy("Regions Group", "South West");
			}));
	});


	it("Should show hint text", () => {
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

		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "");

		Logger.Log("Check validation error if region is not selected");
		addRegionPage.nextStep();
		validationComponent.hasValidationError("Select region");

		cy.excuteAccessibilityTests();

		Logger.Log("Select valid region");
		addRegionPage
			.withRegion("London")
			.nextStep();

		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London");

		Logger.Log("Create a valid concern");
		createConcernPage
			.withConcernType("Governance")
			.withConcernRating("Amber-Green")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.Log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London")
			.hasConcernRiskRating("Amber Green")
			.hasConcernType("Governance");

		createConcernPage.nextStep();

		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London")
			.hasConcernRiskRating("Amber Green")
			.hasConcernType("Governance");

		Logger.Log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red").nextStep();

		Logger.Log(
			"Check Trust, concern and risk to trust details are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails("Ashton West End Primary Academy")
			.hasManagedBy("Regions Group", "London")
			.hasConcernType("Governance")
			.hasConcernRiskRating("Amber Green")
			.hasRiskToTrust("Red");

		Logger.Log("Add concern details with valid text limit");
		addConcernDetailsPage.withIssue("This is an issue").createCase();


		Logger.Log("Adding Notice To Improve");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("Srma");
		AddToCasePage.getAddToCaseBtn().click();



		Logger.Log("Filling out the SRMA form");
		editSrmaPage
			.withStatus("TrustConsidering")
			.withDayTrustContacted("22")
			.withMonthTrustContacted("10")
			.withYearTrustContacted("2022")
			.withNotes("This is my notes")
			.save();

		Logger.Log("Add optional SRMA fields on the view page");
		actionSummaryTable.getOpenAction("SRMA").then((row) => {
			row.hasName("SRMA");
			row.hasStatus("Trust considering");
			row.select();
		});

		Logger.Log("Checking accessibility on View SRMA");
		cy.excuteAccessibilityTests();

		Logger.Log("Configure reason");

		viewSrmaPage.addReason();
		editSrmaPage.verifyTextHint();

	});



});