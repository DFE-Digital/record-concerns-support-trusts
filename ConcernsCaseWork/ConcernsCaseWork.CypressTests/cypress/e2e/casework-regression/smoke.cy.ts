import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";
import { EditFinancialPlanPage } from "../../pages/caseActions/financialPlan/editFinancialPlanPage";
import { EditDecisionPage } from "../../pages/caseActions/decision/editDecisionPage";
import { EditSrmaPage } from "../../pages/caseActions/srma/editSrmaPage";
import { EditNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/editNtiUnderConsiderationPage";
import { Logger } from "../../common/logger";
import { ViewSrmaPage } from "../../pages/caseActions/srma/viewSrmaPage";
import { ViewFinancialPlanPage } from "../../pages/caseActions/financialPlan/viewFinancialPlanPage";
import { CloseFinancialPlanPage } from "../../pages/caseActions/financialPlan/closeFinancialPlanPage";
import { ViewNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/viewNtiUnderConsiderationPage";
import { CloseNtiUnderConsiderationPage } from "../../pages/caseActions/ntiUnderConsideration/closeNtiUnderConsiderationPage";
import closeConcernPage from "../../pages/closeConcernPage";
import HomePage from "../../pages/homePage";
import ClosedCasePage from "../../pages/closedCasePage";
import { EditNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/editNtiWarningLetterPage";
import { CloseNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/closeNtiWarningLetterPage";
import { ViewNtiWarningLetterPage } from "../../pages/caseActions/ntiWarningLetter/viewNtiWarningLetterPage";
import { EditNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/editNoticeToImprovePage";
import { ViewNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/viewNoticeToImprovePage";
import { CloseNoticeToImprovePage } from "../../pages/caseActions/noticeToImprove/closeNoticeToImprovePage";
import { ViewDecisionPage } from "../../pages/caseActions/decision/viewDecisionPage";
import { CloseDecisionPage } from "../../pages/caseActions/decision/closeDecisionPage";
import { DecisionOutcomePage } from "../../pages/caseActions/decision/decisionOutcomePage";
import { ViewClosedCasePage } from "cypress/pages/createCase/viewClosedCasePage";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import actionTable from "cypress/pages/caseRows/caseActionTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { EditTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/editTrustFinancialForecastPage";
import { ViewTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/viewTrustFinancialForecastPage";
import { CloseTrustFinancialForecastPage } from "cypress/pages/caseActions/trustFinancialForecast/closeTrustFinancialForecastPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import createCaseSummary from "cypress/pages/createCase/createCaseSummary";
import selectCaseTypePage from "cypress/pages/createCase/selectCaseTypePage";
import { SourceOfConcernExternal } from "cypress/constants/selectorConstants";
import selectCaseDivisionPage from "cypress/pages/createCase/selectCaseDivisionPage";

describe("Smoke - Testing closing of cases when there are case actions and concerns", () => {
	let caseId: string;
	let trustName: string = "Production Smoke Test Trust";
	let now: Date;

	const createCasePage = new CreateCasePage();
	const createConcernPage = new CreateConcernPage();
	const addTerritoryPage = new AddTerritoryPage();
	const addConcernDetailsPage = new AddConcernDetailsPage();
	const addDetailsPage = new AddDetailsPage();

	const editFinancialPlanPage = new EditFinancialPlanPage();
	const viewFinancialPlanPage = new ViewFinancialPlanPage();
	const closeFinancialPlanPage = new CloseFinancialPlanPage();

	const editDecisionPage = new EditDecisionPage();
	const viewDecisionPage = new ViewDecisionPage();
	const closeDecisionPage = new CloseDecisionPage();
	const decisionOutcomePage = new DecisionOutcomePage();

	const editSrmaPage = new EditSrmaPage();
	const viewSrmaPage = new ViewSrmaPage();

	const editNtiUnderConsiderationPage = new EditNtiUnderConsiderationPage();
	const viewNtiUnderConsiderationPage = new ViewNtiUnderConsiderationPage();
	const closeNtiUnderConsiderationPage = new CloseNtiUnderConsiderationPage();

	const editNtiWarningLetterPage = new EditNtiWarningLetterPage();
	const closeNtiWarningLetterPage = new CloseNtiWarningLetterPage();
	const viewNtiWarningLetterPage = new ViewNtiWarningLetterPage();

	const editNtiPage = new EditNoticeToImprovePage();
	const viewNtiPage = new ViewNoticeToImprovePage();
	const closeNtiPage = new CloseNoticeToImprovePage();

	const editTffPage = new EditTrustFinancialForecastPage();
	const viewTffPage = new ViewTrustFinancialForecastPage();
	const closeTffPage = new CloseTrustFinancialForecastPage();

	const viewClosedCasePage = new ViewClosedCasePage();

	beforeEach(() => {
		cy.login();
		// cy.loginWithCredentials();
		now = new Date();

		Logger.log("Create a case");
		createCasePage
			.createCase()
			.withTrustName(trustName)
			.selectOption()
			.confirmOption();

		Logger.log("Create a valid case division");
		selectCaseDivisionPage
			.withCaseDivision("SFSO")
			.continue();

		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasManagedBy("SFSO", "");
	
		Logger.log("Populate territory");
		addTerritoryPage.withTerritory("North and UTC - North East").nextStep();

		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasManagedBy("SFSO", "North and UTC - North East");

		Logger.log("Create a valid concerns case type");
		selectCaseTypePage.withCaseType("Concerns").continue();

		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasManagedBy("SFSO", "North and UTC - North East");

		Logger.log("Create a valid concern");
		createConcernPage
			.withConcernType("Deficit")
			.withConcernRating("Red-Amber")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.log("Check Concern details are correctly populated");
		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber");

		createConcernPage.nextStep();

		Logger.log(
			"Check Trust, concern and risk to trust details are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber")

		Logger.log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red Plus").nextStep();

		Logger.log(
			"Check Trust, concern, risk to trust details and territory are correctly populated"
		);
		createCaseSummary
			.hasTrustSummaryDetails(trustName)
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasConcernType("Deficit")
			.hasConcernRiskRating("Red Amber")
			.hasRiskToTrust("Red Plus");

		Logger.log("Add concern details with valid text limit");
		addConcernDetailsPage
			.withIssue("This is an issue")
			.withCurrentStatus("This is the current status")
			.withCaseAim("This is the case aim")
			.withDeEscalationPoint("This is the de-escalation point")
			.withNextSteps("This is the next steps")
			.withCaseHistory("This is the case history")
			.createCase();

		CaseManagementPage.getCaseIDText().then((id: string) => {
			caseId = id;
		});
	});

	describe("When we have case actions and concerns that have not been closed", () => {
		it("Should raise a validation error for each case action that has not been closed and only allow a case to be closed when they are resolved", () => {
			addAllAllowedCaseActions();

			Logger.log("Checking accessibility on case management");
			cy.excuteAccessibilityTests();

			Logger.log(
				"Validating an error is displayed for each type of case action"
			);

			CaseManagementPage.getCloseCaseBtn().click();

			CaseManagementPage.hasClosedCaseValidationError("Close financial plan")
				.hasClosedCaseValidationError("Close SRMA action")
				.hasClosedCaseValidationError("Close NTI: Under consideration")
				.hasClosedCaseValidationError("Close decisions")
				.hasClosedCaseValidationError("Close concerns")
				.hasClosedCaseValidationError("Close trust financial forecast");

			Logger.log("Checking accessibility on case management error page");
			cy.excuteAccessibilityTests();

			resolveAllAllowedCaseActions();

			closeConcern();

			closeCaseCheckingValidation();
			verifyClosedCaseDetails();

			Logger.log("Verifying the closed case action is displayed");
			viewClosedCasePage
				.hasClosedCaseAction("SRMA")
				.hasClosedCaseAction("Financial Plan")
				.hasClosedCaseAction("NTI Under Consideration")
				.hasClosedCaseAction("Decision: No Decision Types");

			Logger.log("Verifying the closed case actions details are displayed");
			actionTable.getRowByAction("SRMA").then((row) => {
				row
					.hasName("SRMA")
					.hasStatus("SRMA cancelled")
					.hasOpenedDate(toDisplayDate(now))
					.hasClosedDate(toDisplayDate(now));
			});

			actionTable.getRowByAction("Financial Plan").then((row) => {
				row
					.hasName("Financial Plan")
					.hasStatus("Viable plan received")
					.hasOpenedDate(toDisplayDate(now))
					.hasClosedDate(toDisplayDate(now));
			});

			actionTable.getRowByAction("NTI Under Consideration").then((row) => {
				row
					.hasName("NTI Under Consideration")
					.hasStatus("No further action being taken")
					.hasOpenedDate(toDisplayDate(now))
					.hasClosedDate(toDisplayDate(now));
			});

			actionTable.getRowByAction("Decision: No Decision Types").then((row) => {
				row
					.hasName("Decision: No Decision Types")
					.hasStatus("Approved")
					.hasOpenedDate(toDisplayDate(now))
					.hasClosedDate(toDisplayDate(now));
			});

			actionTable
				.getRowByAction("TFF (trust financial forecast)")
				.then((row) => {
					row
						.hasName("TFF (trust financial forecast)")
						.hasStatus("Completed")
						.hasOpenedDate(toDisplayDate(now))
						.hasClosedDate(toDisplayDate(now));
				});
		});

		it("Should raise a validation error for NTI warning letter and only close when the action resolved", () => {
			Logger.log("Adding NTI warning letter");

			CaseManagementPage.getAddToCaseBtn().click();
			AddToCasePage.addToCase("NtiWarningLetter");
			AddToCasePage.getAddToCaseBtn().click();
			editNtiWarningLetterPage.save();

			Logger.log(
				"Validating an error is displayed for NTI warning letter when case is closed"
			);
			CaseManagementPage.getCloseCaseBtn().click();
			CaseManagementPage.hasClosedCaseValidationError(
				"Close NTI: Warning letter"
			);

			Logger.log("Completing NTI Warning Letter");
			actionSummaryTable.getOpenAction("NTI Warning Letter").then((row) => {
				row.select();
			});

			viewNtiWarningLetterPage.close();
			closeNtiWarningLetterPage.withReason("CancelWarningLetter").close();

			closeConcern();
			closeCase();
			verifyClosedCaseDetails();

			Logger.log("Verifying the closed case action is displayed");
			actionTable.getRowByAction("NTI Warning Letter").then((row) => {
				row
					.hasName("NTI Warning Letter")
					.hasStatus("Cancelled")
					.hasOpenedDate(toDisplayDate(now))
					.hasClosedDate(toDisplayDate(now));
			});
		});

		it("Should raise a validation error for Notice To Improve and only close when the action is resolved", () => {
			Logger.log("Adding Notice To Improve");
			CaseManagementPage.getAddToCaseBtn().click();
			AddToCasePage.addToCase("Nti");
			AddToCasePage.getAddToCaseBtn().click();
			editNtiPage.save();

			Logger.log(
				"Validating an error is displayed for Notice To Improve when case is closed"
			);
			CaseManagementPage.getCloseCaseBtn().click();
			CaseManagementPage.hasClosedCaseValidationError(
				"Cancel, lift or close NTI: Notice to improve"
			);

			Logger.log("Completing Notice To Improve");
			actionSummaryTable.getOpenAction("NTI").then((row) => {
				row.select();
			});

			viewNtiPage.close();
			closeNtiPage.close();

			closeConcern();
			closeCase();
			verifyClosedCaseDetails();

			Logger.log("Verifying the closed case action is displayed");
			actionTable.getRowByAction("NTI").then((row) => {
				row
					.hasName("NTI")
					.hasStatus("Closed")
					.hasOpenedDate(toDisplayDate(now))
					.hasClosedDate(toDisplayDate(now));
			});
		});
	});

	function addAllAllowedCaseActions() {
		Logger.log("Adding all allowed case actions");

		Logger.log("Creating a financial plan");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("FinancialPlan");
		AddToCasePage.getAddToCaseBtn().click();
		editFinancialPlanPage.save();

		Logger.log("Creating a decision");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("Decision");
		AddToCasePage.getAddToCaseBtn().click();
		editDecisionPage.save();

		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then((row) => {
				row.select();
			});

		viewDecisionPage.createDecisionOutcome();
		decisionOutcomePage
			.withDecisionOutcomeStatus("Approved")
			.saveDecisionOutcome();

		Logger.log("Creating an SRMA");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("Srma");
		AddToCasePage.getAddToCaseBtn().click();

		editSrmaPage
			.withStatus("TrustConsidering")
			.withDayTrustContacted("05")
			.withMonthTrustContacted("06")
			.withYearTrustContacted("2022")
			.save();

		Logger.log("Creating NTI under consideration");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("NtiUnderConsideration");
		AddToCasePage.getAddToCaseBtn().click();

		editNtiUnderConsiderationPage.save();

		Logger.log("Creating Trust Financial Forecast(TFF)");
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase("TrustFinancialForecast");
		AddToCasePage.getAddToCaseBtn().click();

		editTffPage.save();
	}

	function resolveAllAllowedCaseActions() {
		Logger.log("Resolving all actions");

		Logger.log("Completing SRMA");
		actionSummaryTable.getOpenAction("SRMA").then((row) => {
			row.select();
		});

		viewSrmaPage.addReason();

		editSrmaPage.withReason("Offer Linked").save();

		viewSrmaPage.cancel();

		editSrmaPage.confirmCancelled().save();

		Logger.log("Completing Financial Plan");
		actionSummaryTable.getOpenAction("Financial Plan").then((row) => {
			row.select();
		});

		viewFinancialPlanPage.close();
		closeFinancialPlanPage
			.withPlanReceivedDay("05")
			.withPlanReceivedMonth("06")
			.withPlanReceivedYear("2022")
			.withReasonForClosure("Viable plan received")
			.close();

		Logger.log("Completing NTI Under Consideration");
		actionSummaryTable.getOpenAction("NTI Under Consideration").then((row) => {
			row.select();
		});

		viewNtiUnderConsiderationPage.close();
		closeNtiUnderConsiderationPage.withStatus("NoFurtherAction").close();

		Logger.log("Completing decision");
		actionSummaryTable
			.getOpenAction("Decision: No Decision Types")
			.then((row) => {
				row.select();
			});

		viewDecisionPage.closeDecision();
		closeDecisionPage.closeDecision();

		Logger.log("Completing Trust financial forecast");
		actionSummaryTable
			.getOpenAction("TFF (trust financial forecast)")
			.then((row) => {
				row.select();
			});

		viewTffPage.close();
		closeTffPage.close();
	}

	function closeConcern() {
		Logger.log("Closing concern");
		CaseManagementPage.closeConcern();

		Logger.log("Checking accessibility on closing concern");
		cy.excuteAccessibilityTests();

		closeConcernPage.confirmCloseConcern();
	}

	function closeCaseCheckingValidation() {
		CaseManagementPage.getCaseIDText().then((caseId) => {
			Logger.log("Closing case");
			CaseManagementPage.getCloseCaseBtn().click();

			Logger.log("Validating that a rationale for closure must be entered");
			CaseManagementPage.getCloseCaseBtn().click();
			CaseManagementPage.hasValidationError(
				"Rationale for closure is required"
			);

			Logger.log("Checking accessibility on Close case");
			cy.excuteAccessibilityTests();

			Logger.log("Validating rationale for closure is 200 characters");
			CaseManagementPage.withRationaleForClosureExceedingLimit();
			CaseManagementPage.getCloseCaseBtn().click();
			CaseManagementPage.hasValidationError(
				"Rationale for closure must be 200 characters or less"
			);

			CaseManagementPage.withRationaleForClosure("Closing case");
			CaseManagementPage.getCloseCaseBtn().click();

			Logger.log("Viewing case is closed");
			HomePage.getClosedCasesBtn().click();
			ClosedCasePage.getClosedCase(caseId);

			Logger.log("Checking accessibility on the closed case view");
			cy.excuteAccessibilityTests();
		});
	}

	function closeCase() {
		CaseManagementPage.getCaseIDText().then((caseId) => {
			Logger.log("Closing case");
			CaseManagementPage.getCloseCaseBtn().click();

			CaseManagementPage.withRationaleForClosure("Closing case");
			CaseManagementPage.getCloseCaseBtn().click();

			Logger.log("Viewing case is closed");
			HomePage.getClosedCasesBtn().click();

			Logger.log("Checking accessibility on closed case");
			cy.excuteAccessibilityTests();

			ClosedCasePage.getClosedCase(caseId);
		});
	}

	function verifyClosedCaseDetails() {
		Logger.log("Validate Closed Case row has correct details");
		caseworkTable.getRowByCaseId(caseId).then((row) => {
			row
				.hasCaseId(caseId)
				.hasCreatedDate(toDisplayDate(now))
				.hasClosedDate(toDisplayDate(now))
				.hasTrust(trustName)
				.hasConcern("Deficit");

			Logger.log("Checking accessibility on Closed Case Details");
			cy.excuteAccessibilityTests();

			row.select();
		});

		Logger.log("Validate Closed Case has correct details");
		viewClosedCasePage
			.hasConcerns("Deficit")
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasIssue("This is an issue")
			.hasCurrentStatus("This is the current status")
			.hasCaseAim("This is the case aim")
			.hasDeEscalationPoint("This is the de-escalation point")
			.hasNextSteps("This is the next steps")
			.hasCaseHistory("This is the case history")
			.hasRationaleForClosure("Closing case");

		Logger.log("Checking accessibility on close case details");
		cy.excuteAccessibilityTests();
	}
});
