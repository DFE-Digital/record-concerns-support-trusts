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

describe("Editing a case", () =>
{
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

    it(("Should create a case with only required fields"), () => {
        Logger.Log("Create a case");
        createCasePage
            .createCase()
            .withTrustName("Ashton West End Primary Academy")
            .selectOption()
            .confirmOption();

        Logger.Log("Create a valid concern");
        createConcernPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red")
            .withMeansOfRefferal("External")
            .addConcern();

        Logger.Log("Check Concern details are correctly populated");
        createConcernPage
            .nextStep();

        Logger.Log("Populate risk to trust");
        addDetailsPage
            .withRating("Red-Plus")
            .nextStep();

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory("North and UTC - North East")
            .nextStep();

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
            .hasRiskToTrust("Red Plus")
            .hasDirectionOfTravel("Deteriorating")
            .hasConcerns("Financial: Deficit", ["Red"])
            .hasTerritory("North and UTC - North East")
            .hasIssue("This is an issue");


        Logger.Log("Edit risk to trust");
        caseManagementPage
             .editRiskToTrust();

        editRiskToTrust
            .hasRiskToTrust("Red-Plus");

        Logger.Log("Checking accessibility on edit risk to trust");
        cy.excuteAccessibilityTests();
        
        editRiskToTrust
            .withRiskToTrust("Red")
            .apply();

        Logger.Log("Edit direction of travel");
        caseManagementPage
            .editDirectionOfTravel();

        editDirectionOfTravel
            .hasDirectionOfTravel("Deteriorating");

        Logger.Log("Checking accessibility on direction of travel");
        cy.excuteAccessibilityTests();

        editDirectionOfTravel
            .withDirectionOfTravel("Improving")
            .apply();

        Logger.Log("Edit a concern");
        caseManagementPage
            .editConcern();

        addDetailsPage
            .hasRating("Red");

        Logger.Log("Checking accessibility on edit concern");
        cy.excuteAccessibilityTests();

        addDetailsPage
            .withRating("Amber-Green")
            .apply();

        Logger.Log("Edit a territory")
        caseManagementPage
            .editTerritory();

        addTerritoryPage
            .hasTerritory("North_And_Utc__North_East");

        Logger.Log("Checking accessibility on edit territory");
        cy.excuteAccessibilityTests();

        addTerritoryPage
            .withTerritory("North_And_Utc__North_West")
            .apply();


        Logger.Log("Edit Issue")
        caseManagementPage
            .showAllConcernDetails()
            .editIssue();

        editIssuePage
            .hasIssue("This is an issue")
            .withExceedingTextLimit()
            .apply()
            .hasValidationError("Issue must be 2000 characters or less");

        Logger.Log("Checking accessibility on edit issue");
        cy.excuteAccessibilityTests();

        cy.waitForJavascript();

        editIssuePage
            .withIssue("New Issue")
            .apply();

        Logger.Log("Edit Current Status")
        caseManagementPage
            .editCurrentStatus();

        editCurrentStatusPage
            .hasCurrentStatus("This is the current status")
            .withExceedingTextLimit()
            .apply()
            .hasValidationError("Current status must be 4000 characters or less");

        Logger.Log("Checking accessibility on edit current status");
        cy.excuteAccessibilityTests();

        cy.waitForJavascript();

        editCurrentStatusPage
            .withCurrentStatus("New Status")
            .apply();


        Logger.Log("Edit Case Aim")
        caseManagementPage
            .editCaseAim();

        editCaseAimPage
            .hasCaseAim("This is the case aim")
            .withExceedingTextLimit()
            .apply()
            .hasValidationError("Case aim must be 1000 characters or less");

        Logger.Log("Checking accessibility on edit case aim");
        cy.excuteAccessibilityTests();

        cy.waitForJavascript();

        editCaseAimPage
            .withCaseAim("New Case aim")
            .apply();

        Logger.Log("Edit Deescalation point")
        caseManagementPage
            .editDeEscalationPoint();

        editDeEscalationPage
            .hasDeescalationPoint("This is the de-escalation point")
            .withExceedingTextLimit()
            .apply()
            .hasValidationError("De-escalation point must be 1000 characters or less");

        Logger.Log("Checking accessibility on edit de-escalation point");
        cy.excuteAccessibilityTests();

        cy.waitForJavascript();

        editDeEscalationPage
            .withDeescalationPoint("New de-descalation point")
            .apply();


        Logger.Log("Edit next steps")
        caseManagementPage
            .editNextSteps();

        editNextStepsPage
            .hasNextSteps("This is the next steps")
            .withExceedingTextLimit()
            .apply()
            .hasValidationError("Next steps must be 4000 characters or less");

        Logger.Log("Checking accessibility on edit next steps");
        cy.excuteAccessibilityTests();

        cy.waitForJavascript();

        editNextStepsPage
            .withNextSteps("New next step")
            .apply();


        Logger.Log("Edit Case history")
        caseManagementPage
            .editCaseHistory();

        editCaseHistoryPage
            .hasCaseHistory("This is the case history")
            .withExceedingTextLimit()
            .apply()
            .hasValidationError("Case history must be 4300 characters or less");

        Logger.Log("Checking accessibility on edit case history");
        cy.excuteAccessibilityTests();

        cy.waitForJavascript();

        editCaseHistoryPage
            .withCaseHistory("New case history")
            .apply();
        
        Logger.Log("Verify detailes have been changed")
        caseManagementPage
            .hasRiskToTrust("Red")
            .hasDirectionOfTravel("Improving")
            .hasConcerns("Financial: Deficit", ["Amber", "Green"])
            .hasTerritory("North and UTC - North West")
            .hasIssue("New Issue")
            .hasCurrentStatus("New Status")
            .hasCaseAim("New Case aim")
            .hasDeEscalationPoint("New de-descalation point")  
            .hasNextSteps("New next step")
            .hasCaseHistory("New case history");
    });

    it("Should raise a validation error if do not select a case action", () => 
    {
        cy.basicCreateCase();

        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.getAddToCaseBtn().click();
        AddToCasePage.hasValidationError("Please select an action to add.");

        Logger.Log("Checking accessibility on when a not selecting a case action");
        cy.excuteAccessibilityTests();
    });
});