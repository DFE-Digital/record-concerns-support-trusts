import { Logger } from "cypress/common/logger";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import { SelectCaseTypePage } from "cypress/pages/createCase/selectCaseTypePage";
import { EnvUsername } from "cypress/constants/cypressConstants";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";
import { EditSrmaPage } from "cypress/pages/caseActions/srma/editSrmaPage";
import { ViewSrmaPage } from "cypress/pages/caseActions/srma/viewSrmaPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";

describe("Creating a case", () => {
    let email: string;
    let name: string;
    const selectCaseTypePage = new SelectCaseTypePage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    const createCasePage = new CreateCasePage();

    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();

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

        Logger.Log("Verify case actions for non concerns");
        CaseManagementPage.getAddToCaseBtn().click();
        
        AddToCasePage.hasActions([
            "Decision", 
            "SRMA (School Resource Management Adviser)", 
            "TFF (Trust Financial Forecast)"
        ]);

        Logger.Log("Create an SRMA on non concerns");

        AddToCasePage.addToCase('Srma')
        AddToCasePage.getAddToCaseBtn().click();

        editSrmaPage
            .withStatus("TrustConsidering")
            .withDayTrustContacted("05")
            .withMonthTrustContacted("06")
            .withYearTrustContacted("2022")
            .withNotes("This is my notes")
            .save();

        actionSummaryTable
			.getOpenAction("SRMA")
			.then(row =>
			{
				row.hasName("SRMA")
				row.hasStatus("Trust considering")
				row.select();
			});

        viewSrmaPage
            .hasStatus("Trust considering")
            .hasDateTrustContacted("05 June 2022")
            .hasNotes("This is my notes");
    });
});