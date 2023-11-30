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
		Logger.log("Create a case");
		createCasePage.createCase().withTrustName(trustName).selectOption().confirmOption();

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

		Logger.log("Populate territory");
		addTerritoryPage.withTerritory(territory).nextStep();

		Logger.log("Create a valid Non-concern case type");
		selectCaseTypePage.withCaseType("NonConcerns").continue();

		Logger.log("Checking accessibility on non concerns confirmation page");
		cy.excuteAccessibilityTests();

		Logger.log("Add non concerns case");
		addConcernDetailsPage.createCase();

		Logger.log("Verify case details");
		caseManagementPage
			.hasTrust(trustName)
			.hasManagedBy("SFSO", territory)
			.hasCaseOwner(name);

		Logger.log("Ensure we cannot see the narritive fields");
		caseManagementPage.hasNoCaseNarritiveFields();

		Logger.log("Verify case actions for non concerns");
		CaseManagementPage.getAddToCaseBtn().click();

		AddToCasePage.hasActions([
			"Decision",
			"SRMA (School Resource Management Adviser)",
			"TFF (trust financial forecast)",
		]);

		Logger.log("Create an SRMA on non concerns");

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

		Logger.log("Closing SRMA");
		viewSrmaPage.addReason();

		editSrmaPage.withReason("Offer Linked").save();

		viewSrmaPage.cancel();

		editSrmaPage.confirmCancelled().save();

		CaseManagementPage.getCaseIDText().then((caseId: string) => {
			closeCase(caseId);
			verifyClosedCaseDetails(caseId);
		});

		Logger.log("Verifying the closed case actions details are displayed");
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
			Logger.log("Create a case");
			createCasePage.createCase().withTrustName(trustName).selectOption().confirmOption();
	
			Logger.log("Create a valid case division");
			selectCaseDivisionPage
				.withCaseDivision("SFSO")
				.continue();
	
			Logger.log("Populate territory");
			addTerritoryPage.withTerritory(territory).nextStep();
	
			Logger.log("Create a valid Non-concern case type");
			selectCaseTypePage.withCaseType("NonConcerns").continue();
	
			Logger.log("Add non concerns case");
			addConcernDetailsPage.createCase();
	
			Logger.log("Add another concern after case creation");
			caseManagementPage.addAnotherConcernForNonConcern();
	
			Logger.log("Checking accessibility on adding concern page");
			cy.excuteAccessibilityTests();
	
			Logger.log("Attempt to create an invalid concern");
			createConcernPage
				.addConcern()
				.hasValidationError("Select concern type")
				.hasValidationError("Select concern risk rating")
				.hasValidationError("Select means of referral");
	
			Logger.log("Create an invalid sub concern");
			createConcernPage
				.withConcernType("Deficit")
				.withConcernRating("Red-Amber")
				.withMeansOfReferral(SourceOfConcernExternal)
				.addConcern();
	
			Logger.log("Adding another concern during case creation");
			createConcernPage
				.addAnotherConcern()
				.withConcernType("Financial compliance")
				.withConcernRating("Amber-Green")
				.withMeansOfReferral(SourceOfConcernInternal)
				.addConcern()
				.nextStep();
	
			Logger.log("Check unpopulated risk to trust throws validation error");
			addConcernDetailsPage
				.nextStep()
				.hasValidationError("Select risk to trust rating");
	
			createConcernPage.withConcernRating("Red Plus").nextStep();
	
			Logger.log("Checking accessibility on create case concern page");
			cy.excuteAccessibilityTests();
	
			Logger.log("Validate unpopulated concern details");
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
				.hasConcerns("Financial compliance", ["Amber", "Green"])
				.hasConcerns("Deficit", ["Red", "Amber"])
				.hasNumberOfConcerns(2)
				.hasManagedBy("SFSO", "North and UTC - North East")
				.hasIssue("This is an issue")
				.hasCurrentStatus("This is the current status")
				.hasCaseAim("This is the case aim")
				.hasDeEscalationPoint("This is the de-escalation point")
				.hasNextSteps("This is the next steps")
				.hasCaseHistory("This is the case history");
	
			Logger.log("Verify the means of referral is set");
			caseManagementPage.getCaseIDText().then((caseId) => {
				concernsApi.get(parseInt(caseId)).then((response) => {
					var ids = response.map((r) => r.meansOfReferralId);
					expect(ids).to.contain(1);
					expect(ids).to.contain(2);
				});
			});
		});

		describe("When we cancel case creation of a different case in the middle", () =>
		{
			it("Should create the concern against the correct case and trust", () =>
			{
				Logger.log("Create a case");
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

					Logger.log("Create a case for an alternative trust");
					createCasePage.createCase().withTrustName(alternativeTrustName).selectOption().confirmOption();
					
					Logger.log("Create a concerns case for our original trust ensuring the data is correct");
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

					Logger.log("Exit early again and create another concern");

					cy.visit(`/case/${caseId}/management`);
					caseManagementPage.addAnotherConcernForNonConcern();

					Logger.log("It should show us the trust for the case we are on");

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East");

					createConcernPage
						.withConcernType("Deficit")
						.withConcernRating("Red-Amber")
						.withMeansOfReferral(SourceOfConcernExternal)
						.addConcern();

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East")
						.hasConcernType("Deficit")
						.hasConcernRiskRating("Red Amber");

					createConcernPage.nextStep();

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East")
						.hasConcernType("Deficit")
						.hasConcernRiskRating("Red Amber");

					createConcernPage.withConcernRating("Red Plus").nextStep();

					createCaseSummary
						.hasTrustSummaryDetails(trustName)
						.hasManagedBy("SFSO", "North and UTC - North East")
						.hasConcernType("Deficit")
						.hasConcernRiskRating("Red Amber")
						.hasRiskToTrust("Red Plus");

					addConcernDetailsPage
						.withIssue("This is an issue").createCase();

					Logger.log("It should create just one concern against the correct trust");
					caseManagementPage
						.hasTrust(trustName)
						.hasRiskToTrust("Red Plus")
						.hasConcerns("Deficit", ["Red", "Amber"])
						.hasNumberOfConcerns(1);
				});
			});
		});
	});

	function closeCase(caseId: string) {
		Logger.log("Closing case");
		CaseManagementPage.getCloseCaseBtn().click();

		CaseManagementPage.withRationaleForClosure("Closing non concerns case");
		CaseManagementPage.getCloseCaseBtn().click();

		Logger.log("Viewing case is closed");
		homePage.getClosedCasesBtn().click();
		ClosedCasePage.getClosedCase(caseId);
	}

	function verifyClosedCaseDetails(caseId: string) {
		Logger.log("Validate Closed Case row has correct details");
		caseworkTable.getRowByCaseId(caseId).then((row) => {
			row
				.hasCaseId(caseId)
				.hasCreatedDate(toDisplayDate(now))
				.hasClosedDate(toDisplayDate(now))
				.hasTrust(trustName)
				.select();
		});

		Logger.log("Validate Closed Case has correct details");
		viewClosedCasePage
			.hasTrust(trustName)
			.hasManagedBy("SFSO", territory)
			.hasCaseOwner(name)
			.hasRationaleForClosure("Closing non concerns case")
			.hasNoCaseNarritiveFields();
	}
});
