import { Logger } from "cypress/common/logger";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import { SelectCaseTypePage } from "cypress/pages/createCase/selectCaseTypePage";

describe("Creating a case", () => {
    const selectCaseTypePage = new SelectCaseTypePage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    beforeEach(() => {
        cy.login();
        cy.visit(Cypress.env('url') + '/case/create');
    });

    it("Should validate adding a case", () => {
        const trustName = "Ashton West End Primary Academy";
        const territory = "North and UTC - North East";
        const caseHistoryData = "This is the case history";
        Logger.Log("Create a case");
        selectCaseTypePage
            .withTrustName(trustName)
            .selectOption()
            .confirmOption();

        Logger.Log("Attempt to create an invalid create case type");
        selectCaseTypePage
            .confirmOption()
            .hasTrustSummaryDetails("Select Case type")
        cy.waitForJavascript();

        Logger.Log("Create a valid Non-concern case type");
        selectCaseTypePage
            .withNonConcernCaseType("NonConcerns")
            .addConcern();
        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory(territory)
            .nextStep();

        Logger.Log("Validate unpopulated concern details");
        addConcernDetailsPage
            .withCaseHistoryExceedingLimit()
            .createCase()
            .hasValidationError("Case history must be 4300 characters or less");

        Logger.Log("Add concern case details with valid text limit");
        addConcernDetailsPage
            .withCaseHistory(caseHistoryData)
            .createCase();

        Logger.Log("Verify case details");
        caseManagementPage
            .hasTrust(trustName)
            .hasTerritory(territory)
            .hasCaseHistory(caseHistoryData);
    });
});