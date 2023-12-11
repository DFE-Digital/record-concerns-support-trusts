import { Logger } from "cypress/common/logger";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import EditRiskToTrustPage from "cypress/pages/createCase/editRiskToTrustPage";
import EditDirectionOfTravelPage from "cypress/pages/createCase/editDirectionOfTravelPage";
import EditIssuePage from "cypress/pages/createCase/editIssuePage";
import EditCurrentStatusPage from "cypress/pages/createCase/editCurrentStatusPage";
import EditCaseAimPage from "cypress/pages/createCase/editCaseAimPage";
import EditDeEscalationPointPage from "cypress/pages/createCase/editDeescalationPointPage";
import EditNextStepsPage from "cypress/pages/createCase/editNextStepsPage";
import EditCaseHistoryPage from "cypress/pages/createCase/editCaseHistoryPage";
import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";
import selectCaseTypePage from "cypress/pages/createCase/selectCaseTypePage";
import { SourceOfConcernExternal } from "cypress/constants/selectorConstants";
import selectCaseDivisionPage from "cypress/pages/createCase/selectCaseDivisionPage";

describe("Editing a case", () => {
	const createCasePage = new CreateCasePage();
	const createConcernPage = new CreateConcernPage();
	const addDetailsPage = new AddDetailsPage();
	const addTerritoryPage = new AddTerritoryPage();
	const addConcernDetailsPage = new AddConcernDetailsPage();
	const editRiskToTrust = new EditRiskToTrustPage();
	const editDirectionOfTravel = new EditDirectionOfTravelPage();
	const editIssuePage = new EditIssuePage();
	const editCurrentStatusPage = new EditCurrentStatusPage();
	const editCaseAimPage = new EditCaseAimPage();
	const editDeEscalationPage = new EditDeEscalationPointPage();
	const editNextStepsPage = new EditNextStepsPage();
	const editCaseHistoryPage = new EditCaseHistoryPage();

	beforeEach(() => {
		cy.login();
	});

	it("Should be able to edit a case", () => {
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
        selectCaseTypePage
            .withCaseType("Concerns")
            .continue();

		Logger.log("Create a valid concern");
		createConcernPage
			.withConcernType("Deficit")
			.withConcernRating("Red")
			.withMeansOfReferral(SourceOfConcernExternal)
			.addConcern();

		Logger.log("Check Concern details are correctly populated");
		createConcernPage.nextStep();

		Logger.log("Populate risk to trust");
		addDetailsPage.withRiskToTrust("Red Plus").nextStep();

		

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
			.hasRiskToTrust("Red Plus")
			.hasDirectionOfTravel("Deteriorating")
			.hasConcerns("Deficit", ["Red"])
			.hasManagedBy("SFSO", "North and UTC - North East")
			.hasIssue("This is an issue");

		Logger.log("Edit risk to trust");
		caseManagementPage.editRiskToTrust();

		editRiskToTrust.hasRiskToTrust("Red Plus");

		Logger.log("Checking accessibility on edit risk to trust");
		cy.excuteAccessibilityTests();

		editRiskToTrust.withRiskToTrust("Red").apply();

		Logger.log("Edit direction of travel");
		caseManagementPage.editDirectionOfTravel();

		editDirectionOfTravel.hasDirectionOfTravel("Deteriorating");

		Logger.log("Checking accessibility on direction of travel");
		cy.excuteAccessibilityTests();

		editDirectionOfTravel.withDirectionOfTravel("Improving").apply();

		Logger.log("Edit a concern");
		caseManagementPage.editConcern();

		addDetailsPage.hasRating("Red");

		Logger.log("Checking accessibility on edit concern");
		cy.excuteAccessibilityTests();

		addDetailsPage.withRiskToTrust("Amber-Green").apply();

		Logger.log("Edit a territory");
		caseManagementPage.editManagedBy();

		addTerritoryPage.hasTerritory("North and UTC - North East");

		Logger.log("Checking accessibility on edit territory");
		cy.excuteAccessibilityTests();

		addTerritoryPage.withTerritory("North and UTC - North West").apply();

		Logger.log("Edit Issue");
		caseManagementPage.showAllConcernDetails().editIssue();

		editIssuePage.hasIssue("This is an issue");

		editIssuePage.clearIssue().apply().hasValidationError("Issue is required");

		editIssuePage
			.withExceedingTextLimit()
			.apply()
			.hasValidationError("Issue must be 2000 characters or less");

		// Ensure the correct character count when new lines are used
		editIssuePage
			.withIssue("Testing \n the character count \n with \n\n\n new lines")
			.hasCharacterCountMessage("You have 1,945 characters remaining");

		Logger.log("Checking accessibility on edit issue");
		cy.excuteAccessibilityTests();

		editIssuePage.withIssue("New Issue").apply();

		Logger.log("Edit Current Status");
		caseManagementPage.editCurrentStatus();

		editCurrentStatusPage
			.hasCurrentStatus("This is the current status")
			.withExceedingTextLimit()
			.apply()
			.hasValidationError("Current status must be 4000 characters or less");

		Logger.log("Checking accessibility on edit current status");
		cy.excuteAccessibilityTests();

		editCurrentStatusPage.withCurrentStatus("New Status").apply();

		Logger.log("Edit Case Aim");
		caseManagementPage.editCaseAim();

		editCaseAimPage
			.hasCaseAim("This is the case aim")
			.withExceedingTextLimit()
			.apply()
			.hasValidationError("Case aim must be 1000 characters or less");

		Logger.log("Checking accessibility on edit case aim");
		cy.excuteAccessibilityTests();

		editCaseAimPage.withCaseAim("New Case aim").apply();

		Logger.log("Edit Deescalation point");
		caseManagementPage.editDeEscalationPoint();

		editDeEscalationPage
			.hasDeescalationPoint("This is the de-escalation point")
			.withExceedingTextLimit()
			.apply()
			.hasValidationError(
				"De-escalation point must be 1000 characters or less"
			);

		Logger.log("Checking accessibility on edit de-escalation point");
		cy.excuteAccessibilityTests();

		editDeEscalationPage
			.withDeescalationPoint("New de-descalation point")
			.apply();

		Logger.log("Edit next steps");
		caseManagementPage.editNextSteps();

		editNextStepsPage
			.hasNextSteps("This is the next steps")
			.withExceedingTextLimit()
			.apply()
			.hasValidationError("Next steps must be 4000 characters or less");

		Logger.log("Checking accessibility on edit next steps");
		cy.excuteAccessibilityTests();

		editNextStepsPage.withNextSteps("New next step").apply();

		Logger.log("Edit Case history");
		caseManagementPage.editCaseHistory();

		editCaseHistoryPage
			.hasCaseHistory("This is the case history")
			.withExceedingTextLimit()
			.apply()
			.hasValidationError("Case notes must be 4300 characters or less");

		Logger.log("Checking accessibility on edit case history");
		cy.excuteAccessibilityTests();

		editCaseHistoryPage.withCaseHistory("New case history").apply();

		Logger.log("Verify details have been changed");
		caseManagementPage
			.hasRiskToTrust("Red")
			.hasDirectionOfTravel("Improving")
			.hasConcerns("Deficit", ["Amber", "Green"])
			.hasManagedBy("SFSO", "North and UTC - North West")
			.hasIssue("New Issue")
			.hasCurrentStatus("New Status")
			.hasCaseAim("New Case aim")
			.hasDeEscalationPoint("New de-descalation point")
			.hasNextSteps("New next step")
			.hasCaseHistory("New case history");

		Logger.log("Ensure that we can remove all optional information");
		caseManagementPage.editCurrentStatus();
		editCurrentStatusPage.withCurrentStatus("").apply();
		caseManagementPage.editCaseAim();
		editCaseAimPage.withCaseAim("").apply();
		caseManagementPage.editDeEscalationPoint();
		editDeEscalationPage.withDeescalationPoint("").apply();
		caseManagementPage.editNextSteps();
		editNextStepsPage.withNextSteps("").apply();
		caseManagementPage.editCaseHistory();
		editCaseHistoryPage.withCaseHistory("").apply();

		Logger.log("Verify details have been changed");
		caseManagementPage
			.hasIssue("New Issue")
			.hasEmptyCurrentStatus()
			.hasEmptyCaseAim()
			.hasEmptyDeEscalationPoint()
			.hasEmptyNextSteps()
			.hasEmptyCaseHistory();

	});

	it("Should raise a validation error if do not select a case action", () => {
		cy.basicCreateCase();

		CaseManagementPage.getAddToCaseBtn().click();

		AddToCasePage.hasActions([
            "Decision",
			"Financial plan",
            "NTI: Under consideration",
            "NTI: Warning letter",
            "NTI: Notice to improve",
            "SRMA (School Resource Management Adviser)",
			"TFF (trust financial forecast)"
        ]);

		AddToCasePage.getAddToCaseBtn().click();
		AddToCasePage.hasValidationError("Select an action or decision");

		Logger.log("Checking accessibility on when a not selecting a case action");
		cy.excuteAccessibilityTests();
	});
});
