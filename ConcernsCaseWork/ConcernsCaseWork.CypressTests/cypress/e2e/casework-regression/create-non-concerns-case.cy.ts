import { Logger } from "cypress/common/logger";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import { EnvUsername } from "cypress/constants/cypressConstants";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";
import { EditSrmaPage } from "cypress/pages/caseActions/srma/editSrmaPage";
import { ViewSrmaPage } from "cypress/pages/caseActions/srma/viewSrmaPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import ClosedCasePage from "../../pages/closedCasePage";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { ViewClosedCasePage } from "cypress/pages/createCase/viewClosedCasePage";
import actionTable from "cypress/pages/caseRows/caseActionTable";
import concernsApi from "cypress/api/concernsApi";
import selectCaseTypePage from "cypress/pages/createCase/selectCaseTypePage";
import {
	SourceOfConcernExternal,
	SourceOfConcernInternal,
} from "cypress/constants/selectorConstants";
import selectCaseDivisionPage from "cypress/pages/createCase/selectCaseDivisionPage";
import createCaseSummary from "cypress/pages/createCase/createCaseSummary";
import homePage from "../../pages/homePage";
import headerComponent from "cypress/pages/header";

describe("Creating a non concerns case", () => {
	let email: string;
	let name: string;
	const addTerritoryPage = new AddTerritoryPage();
	const addConcernDetailsPage = new AddConcernDetailsPage();
	const createCasePage = new CreateCasePage();

	const editSrmaPage = new EditSrmaPage();
	const viewSrmaPage = new ViewSrmaPage();
	const viewClosedCasePage = new ViewClosedCasePage();
	const createConcernPage = new CreateConcernPage();

	const trustName = "Ashton West End Primary Academy";
	const alternativeTrustName = "Denton West End Primary School";
	const territory = "North and UTC - North East";

	let now: Date;

	beforeEach(() => {
		cy.login();
		email = Cypress.env(EnvUsername);
		name = email.split("@")[0];
		now = new Date();
	});

	it("Should validate adding a case", () => {
		Logger.Log("Create a case");
		createCasePage.createCase().withTrustName(trustName).selectOption().confirmOption();

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

		Logger.Log("Populate territory");
		addTerritoryPage.withTerritory(territory).nextStep();

		Logger.Log("Create a valid Non-concern case type");
		selectCaseTypePage.withCaseType("NonConcerns").continue();

		Logger.Log("Checking accessibility on non concerns confirmation page");
		cy.excuteAccessibilityTests();

		Logger.Log("Add non concerns case");
		addConcernDetailsPage.createCase();

		Logger.Log("Verify case details");
		caseManagementPage
			.hasTrust(trustName)
			.hasManagedBy("SFSO", territory)
			.hasCaseOwner(name);

		Logger.Log("Ensure we cannot see the narritive fields");
		caseManagementPage.hasNoCaseNarritiveFields();

		Logger.Log("Verify case actions for non concerns");
		CaseManagementPage.getAddToCaseBtn().click();

		AddToCasePage.hasActions([
			"Decision",
			"SRMA (School Resource Management Adviser)",
			"TFF (trust financial forecast)",
		]);

		Logger.Log("Create an SRMA on non concerns");

		AddToCasePage.addToCase("Srma");
		AddToCasePage.getAddToCaseBtn().click();

		editSrmaPage
			.withStatus("TrustConsidering")
			.withDayTrustContacted("05")
			.withMonthTrustContacted("06")
			.withYearTrustContacted("2022")
			.withNotes("This is my notes")
			.save();

		actionSummaryTable.getOpenAction("SRMA").then((row) => {
			row.hasName("SRMA");
			row.hasStatus("Trust considering");
			row.select();
		});

		viewSrmaPage
			.hasStatus("Trust considering")
			.hasDateTrustContacted("05 June 2022")
			.hasNotes("This is my notes");

		Logger.Log("Closing SRMA");
		viewSrmaPage.addReason();

		editSrmaPage.withReason("Offer Linked").save();

		viewSrmaPage.cancel();

		editSrmaPage.confirmCancelled().save();

		CaseManagementPage.getCaseIDText().then((caseId: string) => {
			closeCase(caseId);
			verifyClosedCaseDetails(caseId);
		});

		Logger.Log("Verifying the closed case actions details are displayed");
		actionTable.getRowByAction("SRMA").then((row) => {
			row
				.hasName("SRMA")
				.hasStatus("SRMA cancelled")
				.hasOpenedDate(toDisplayDate(now))
				.hasClosedDate(toDisplayDate(now));
		});
	});

	describe("Converting non conern to concern case", () =>
	{
		it("Should make the case a concerns case", () => {
			Logger.Log("Create a case");
			createCasePage.createCase().withTrustName(trustName).selectOption().confirmOption();
	
			Logger.Log("Create a valid case division");
			selectCaseDivisionPage
				.withCaseDivision("SFSO")
				.continue();
	
			Logger.Log("Populate territory");
			addTerritoryPage.withTerritory(territory).nextStep();
	
			Logger.Log("Create a valid Non-concern case type");
			selectCaseTypePage.withCaseType("NonConcerns").continue();
	
			Logger.Log("Add non concerns case");
			addConcernDetailsPage.createCase();
	
			Logger.Log("Add another concern after case creation");
			caseManagementPage.addAnotherConcernForNonConcern();
	
			Logger.Log("Checking accessibility on adding concern page");
			cy.excuteAccessibilityTests();
	
			Logger.Log("Attempt to create an invalid concern");
			createConcernPage
				.addConcern()
				.hasValidationError("Select concern type")
				.hasValidationError("Select concern risk rating")
				.hasValidationError("Select means of referral");
	
			createConcernPage
				.withConcernType("Deficit")
				.withConcernRating("Red-Amber")
				.withMeansOfReferral(SourceOfConcernExternal)
				.addConcern();
	
			Logger.Log("Check unpopulated risk to trust throws validation error");
			addConcernDetailsPage
				.nextStep()
				.hasValidationError("Select risk to trust rating");
	
			createConcernPage.withConcernRating("Red-Plus").nextStep();
	
			Logger.Log("Checking accessibility on create case concern page");
			cy.excuteAccessibilityTests();
	
			Logger.Log("Validate unpopulated concern details");
			addConcernDetailsPage
				.withIssueExceedingLimit()
				.withCurrentStatusExceedingLimit()
				.withCaseAimExceedingLimit()
				.withDeEscalationPointExceedingLimit()
				.withNextStepsExceedingLimit()
				.withCaseHistoryExceedingLimit()
				.createCase()
				.hasValidationError("Issue must be 2000 characters or less")
				.hasValidationError("Current status must be 4000 characters or less")
				.hasValidationError("Next steps must be 4000 characters or less")
				.hasValidationError("De-escalation point must be 1000 characters or less")
				.hasValidationError("Case aim must be 1000 characters or less")
				.hasValidationError("Case notes must be 4300 characters or less");
	
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
				.hasNumberOfConcerns(1)
				.hasManagedBy("SFSO", "North and UTC - North East")
				.hasIssue("This is an issue")
				.hasCurrentStatus("This is the current status")
				.hasCaseAim("This is the case aim")
				.hasDeEscalationPoint("This is the de-escalation point")
				.hasNextSteps("This is the next steps")
				.hasCaseHistory("This is the case history");
	
			Logger.Log("Verify the means of referral is set");
			caseManagementPage.getCaseIDText().then((caseId) => {
				concernsApi.get(parseInt(caseId)).then((response) => {
					var ids = response.map((r) => r.meansOfReferralId);
					expect(ids).to.contain(2);
				});
			});
		});

		describe("When we cancel case creation of a different case in the middle", () =>
		{
			it("Should create the concern against the correct case and trust", () =>
			{
				Logger.Log("Create a case");
				createCasePage.createCase().withTrustName(trustName).selectOption().confirmOption();
		
				selectCaseDivisionPage
					.withCaseDivision("SFSO")
					.continue();
		
				addTerritoryPage.withTerritory(territory).nextStep();
				selectCaseTypePage.withCaseType("NonConcerns").continue();
				addConcernDetailsPage.createCase();

				caseManagementPage.getCaseIDText()
				.then((caseId: string) =>
				{
					headerComponent.goToHome();

					Logger.Log("Create a case for an alternative trust");
					createCasePage.createCase().withTrustName(alternativeTrustName).selectOption().confirmOption();
					
					Logger.Log("Create a concerns case for our original trust ensuring the data is correct");
					cy.visit(`/case/${caseId}/management`);
					caseManagementPage.addAnotherConcernForNonConcern();

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East");

					createConcernPage
						.withConcernType("Viability")
						.withConcernRating("Amber-Green")
						.withMeansOfReferral(SourceOfConcernExternal)
						.addConcern();

					Logger.Log("Exit early again and create another concern");

					cy.visit(`/case/${caseId}/management`);
					caseManagementPage.addAnotherConcernForNonConcern();

					Logger.Log("It should show us the trust for the case we are on");

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East");

					createConcernPage
						.withConcernType("Deficit")
						.withConcernRating("Red-Amber")
						.withMeansOfReferral(SourceOfConcernExternal)
						.addConcern();

					Logger.Log("Go back and edit the concern");
					cy.go('back');

					createConcernPage
						.withConcernType("Irregularity")
						.withConcernRating("Red")
						.withMeansOfReferral(SourceOfConcernExternal)
						.addConcern();
						
					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East")
						.hasConcernType("Irregularity")
						.hasConcernRiskRating(["Red"]);

					createConcernPage.nextStep();

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East")
						.hasConcernType("Irregularity")
						.hasConcernRiskRating(["Red"]);

					createConcernPage.withConcernRating("Red-Plus").nextStep();

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East")
						.hasConcernType("Irregularity")
						.hasConcernRiskRating(["Red"])
						.hasRiskToTrust("Red Plus");

					addConcernDetailsPage
						.withIssue("This is an issue").createCase();

					Logger.Log("It should create just one concern against the correct trust");
					caseManagementPage
						.hasTrust(trustName)
						.hasRiskToTrust("Red Plus")
						.hasConcerns("Irregularity", ["Red"])
						.hasNumberOfConcerns(1);
				});
			});
		});
	});

	function closeCase(caseId: string) {
		Logger.Log("Closing case");
		CaseManagementPage.getCloseCaseBtn().click();

		CaseManagementPage.withRationaleForClosure("Closing non concerns case");
		CaseManagementPage.getCloseCaseBtn().click();

		Logger.Log("Viewing case is closed");
		homePage.getClosedCasesBtn().click();
		ClosedCasePage.getClosedCase(caseId);
	}

	function verifyClosedCaseDetails(caseId: string) {
		Logger.Log("Validate Closed Case row has correct details");
		caseworkTable.getRowByCaseId(caseId).then((row) => {
			row
				.hasCaseId(caseId)
				.hasCreatedDate(toDisplayDate(now))
				.hasClosedDate(toDisplayDate(now))
				.hasTrust(trustName)
				.select();
		});

		Logger.Log("Validate Closed Case has correct details");
		viewClosedCasePage
			.hasTrust(trustName)
			.hasManagedBy("SFSO", territory)
			.hasCaseOwner(name)
			.hasRationaleForClosure("Closing non concerns case")
			.hasNoCaseNarritiveFields();
	}
});
