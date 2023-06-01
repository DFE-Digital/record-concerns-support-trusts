import { Logger } from "cypress/common/logger";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import { SelectCaseTypePage } from "cypress/pages/createCase/selectCaseTypePage";
import { EnvUsername } from "cypress/constants/cypressConstants";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import CaseManagementPage from "../../pages/caseMangementPage";
import AddToCasePage from "../../pages/caseActions/addToCasePage";
import { EditSrmaPage } from "cypress/pages/caseActions/srma/editSrmaPage";
import { ViewSrmaPage } from "cypress/pages/caseActions/srma/viewSrmaPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import HomePage from "../../pages/homePage";
import ClosedCasePage from "../../pages/closedCasePage";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import { toDisplayDate } from "cypress/support/formatDate";
import { ViewClosedCasePage } from "cypress/pages/createCase/viewClosedCasePage";
import actionTable from "cypress/pages/caseRows/caseActionTable";

describe("Creating a case", () => {
    let email: string;
    let name: string;
    const selectCaseTypePage = new SelectCaseTypePage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    const createCasePage = new CreateCasePage();
    

    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();
    const viewClosedCasePage = new ViewClosedCasePage();
    const createConcernPage = new CreateConcernPage();


    const trustName = "Ashton West End Primary Academy";
    const territory = "North and UTC - North East";

    let now: Date;

    beforeEach(() => {
        cy.login();
        cy.visit(Cypress.env('url') + '/case/create');
        email = Cypress.env(EnvUsername);
        name = email.split("@")[0];
        now = new Date();
    });

    it.only("Should validate adding a case", () => {
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
            .hasValidationError("Select case type");

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

        Logger.Log("Checking accessibility on non concerns confirmation page");
        cy.excuteAccessibilityTests();

        Logger.Log("Add non concerns case");
        addConcernDetailsPage
            .createCase();

        Logger.Log("Verify case details");
        caseManagementPage
            .hasTrust(trustName)
            .hasTerritory(territory)
            .hasCaseOwner(name);
        
        Logger.Log("Ensure we cannot see the narritive fields");
        caseManagementPage
            .hasNoCaseNarritiveFields();

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
        
        Logger.Log("Closing SRMA");
        viewSrmaPage
            .addReason()

        editSrmaPage
            .withReason("Offer Linked")
            .save();

        viewSrmaPage
            .cancel();

        editSrmaPage
            .confirmCancelled()
            .save();

        CaseManagementPage.getCaseIDText().then((caseId: string) => {
            closeCase(caseId);
            verifyClosedCaseDetails(caseId);
        });

        Logger.Log("Verifying the closed case actions details are displayed");
        actionTable
            .getRowByAction("SRMA")
            .then((row) => {
                row
                    .hasName("SRMA")
                    .hasStatus("SRMA cancelled")
                    .hasOpenedDate(toDisplayDate(now))
                    .hasClosedDate(toDisplayDate(now))
            });
    });

    
    // Logger.Log("Click add concern")
    // caseManagementPage.
    // getAddConcernBtn();

    // Logger.Log("Checking accessibility on adding concern page");
    // cy.excuteAccessibilityTests();

    // Logger.Log("Create a valid concern");
    // createConcernPage
    //     .withConcernType("Financial")
    //     .withSubConcernType("Financial: Deficit")
    //     .withRating("Red-Amber")
    //     .withMeansOfRefferal("External")
    //     .addConcern();

    // Logger.Log("Click next to take you to risk page and Select risk");
    // addConcernDetailsPage.nextStep();
    // createConcernPage

    // .withRating("Red-Amber");
    // addConcernDetailsPage.nextStep();
    //     Logger.Log("Checking accessibility on create case concern page");
    //     cy.excuteAccessibilityTests();

    //     Logger.Log("Populate territory");
    //     addTerritoryPage
    //         .withTerritory(territory)
    //         .nextStep();
    //         Logger.Log("Add concern details with valid text limit");
    //         addConcernDetailsPage
    //             .withIssue("This is an issue")
    //             .withCurrentStatus("This is the current status")
    //             .withCaseAim("This is the case aim")
    //             .withDeEscalationPoint("This is the de-escalation point")
    //             .withNextSteps("This is the next steps")
    //             .withCaseHistory("This is the case history")
    //             .createCase();









    it("Converting non conern to concern case", () => {
        Logger.Log("Create a case");
        createCasePage
            .withTrustName(trustName)
            .selectOption()
            .confirmOption();


        Logger.Log("Create a valid Non-concern case type");
        selectCaseTypePage
            .withCaseType("NonConcerns")
            .continue();

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory(territory)
            .nextStep();

        Logger.Log("Add non concerns case");
        addConcernDetailsPage
            .createCase();

            Logger.Log("Add another concern after case creation");
            caseManagementPage.addAnotherConcern();
    
            Logger.Log("Checking accessibility on adding concern page");
            cy.excuteAccessibilityTests();
    
            Logger.Log("Create a valid concern");
            createConcernPage
                .withConcernType("Financial")
                .withSubConcernType("Financial: Deficit")
                .withRating("Red-Amber")
                .withMeansOfRefferal("External")
                .addConcern();
                Logger.Log("Adding another concern during case creation");
                createConcernPage
                    .addAnotherConcern()
                    .withConcernType("Governance and compliance")
                    .withSubConcernType("Governance and compliance: Compliance")
                    .withRating("Amber-Green")
                    .withMeansOfRefferal("Internal")
                    .addConcern()
                    .nextStep();
            Logger.Log("Click next to take you to risk page and Select risk");
            addConcernDetailsPage.nextStep();
            createConcernPage.withRating("Red-Amber");
            cy.waitForJavascript();
            addConcernDetailsPage.nextStep();
                Logger.Log("Checking accessibility on create case concern page");
                cy.excuteAccessibilityTests();
                Logger.Log("Populate territory");
        
                Logger.Log("Add concern details with valid text limit");
                addConcernDetailsPage
                    .withIssue("This is an issue")
                    .createCase();
        
                Logger.Log("Add another concern after case creation");
                caseManagementPage.addAnotherConcern();
        
                createConcernPage
                    .withConcernType("Irregularity")
                    .withSubConcernType("Irregularity: Irregularity")
                    .withRating("Red-Amber")
                    .withMeansOfRefferal("External")
                    .addConcern();
        
                    // Logger.Log("Add concern details with valid text limit");
                    // addConcernDetailsPage
                    //     .withIssue("This is an issue")
                    //     .withCurrentStatus("This is the current status")
                    //     .withCaseAim("This is the case aim")
                    //     .withDeEscalationPoint("This is the de-escalation point")
                    //     .withNextSteps("This is the next steps")
                    //     .withCaseHistory("This is the case history")
                    //     .createCase();
               
    });











    function closeCase(caseId: string) {
            Logger.Log("Closing case");
            CaseManagementPage.getCloseCaseBtn().click();

            CaseManagementPage.withRationaleForClosure("Closing case");
            CaseManagementPage.getCloseCaseBtn().click();

            Logger.Log("Viewing case is closed");
            HomePage.getClosedCasesBtn().click();
            ClosedCasePage.getClosedCase(caseId);
    }

    function verifyClosedCaseDetails(caseId: string) {

        Logger.Log("Validate Closed Case row has correct details");
        caseworkTable
            .getRowByCaseId(caseId)
            .then((row) => {
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
            .hasTerritory(territory)
            .hasCaseOwner(name)
            .hasNoCaseNarritiveFields();
    }
});