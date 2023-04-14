import { Logger } from "cypress/common/logger";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import concernsApi from "cypress/api/concernsApi";
import caseApi from "cypress/api/caseApi";
describe("Creating a case", () => {
    const createCasePage = new CreateCasePage();
    const createConcernPage = new CreateConcernPage();
    const addDetailsPage = new AddDetailsPage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();

    beforeEach(() => {
        cy.login();
    });

    it.only("Accessbility check for adding a case journey", () => {
        Logger.Log("Accessbility check for Create a case Journey page");
        createCasePage
            .createCase()
        cy.excuteAccessibilityTests()
        createCasePage.withTrustName("Ashton West End Primary Academy")
            .selectOption()
            .confirmOption()

        Logger.Log("Accessbility check for add concern page");
        createConcernPage
        cy.excuteAccessibilityTests();
        createConcernPage.withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .addConcern()
        cy.excuteAccessibilityTests();

        Logger.Log("Populate risk to trust");
        addDetailsPage
            .nextStep()
            .withRating("Red-Plus")
            .nextStep()
        cy.excuteAccessibilityTests();

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory("North and UTC - North East")
            .nextStep();

    });
});
