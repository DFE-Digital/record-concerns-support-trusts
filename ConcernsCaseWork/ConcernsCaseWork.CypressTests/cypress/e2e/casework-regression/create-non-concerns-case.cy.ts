import { Logger } from "cypress/common/logger";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import { SelectCaseTypePage } from "cypress/pages/createCase/selectCaseTypePage";
import { EnvUsername } from "cypress/constants/cypressConstants";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";

describe("Creating a case", () => {
    let email: string;
    let name: string;
    const selectCaseTypePage = new SelectCaseTypePage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    const createCasePage = new CreateCasePage();
    const createConcernPage = new CreateConcernPage();

    beforeEach(() => {
        cy.login();
        cy.visit(Cypress.env('url') + '/case/create');
        email = Cypress.env(EnvUsername);
        name = email.split("@")[0];
    });

    it("Should validate adding a case", () => {
        const trustName = "Ashton West End Primary Academy";
        const territory = "North and UTC - North East";
        const caseHistoryData = "This is the case history";
        Logger.Log("Create a case");
        createCasePage
            .withTrustName(trustName)
            .selectOption()
            .confirmOption();

        Logger.Log("You must select a case error");
        selectCaseTypePage
            .continue()
            .hasValidationError("Select Case type");

        Logger.Log("Checking accessibility on select case type");
        cy.excuteAccessibilityTests();

        Logger.Log("Create a valid Non-concern case type");
        selectCaseTypePage
            .withCaseType("NonConcerns")
            .continue();
        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory(territory)
            .nextStep();

        Logger.Log("Validate adding concern details errors");
        addConcernDetailsPage
            .withCaseHistoryExceedingLimit()
            .createCase()
            .hasValidationError("Case history must be 4300 characters or less");

        Logger.Log("Checking accessibility on non concerns confirmation page");
        cy.excuteAccessibilityTests();

        Logger.Log("Add concern case details with valid text limit");
        addConcernDetailsPage
            .withCaseHistory(caseHistoryData)
            .createCase();

        Logger.Log("Verify case details");
        caseManagementPage
            .hasTrust(trustName)
            .hasTerritory(territory)
            .hasCaseHistory(caseHistoryData)
            .hasCaseOwner(name);

        Logger.Log("Click add concern")
        caseManagementPage.
        getAddConcernBtn();

        Logger.Log("Checking accessibility on adding concern page");
        cy.excuteAccessibilityTests();

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .addConcern();

        Logger.Log("Click next to take you to risk page and Select risk");
        addConcernDetailsPage.nextStep();
        createConcernPage
   
        .withRating("Red-Amber");
        addConcernDetailsPage.nextStep();
            Logger.Log("Checking accessibility on create case concern page");
            cy.excuteAccessibilityTests();

            Logger.Log("Populate territory");
            addTerritoryPage
                .withTerritory(territory)
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
           

    });

    
});