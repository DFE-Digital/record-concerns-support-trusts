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
import concernsApi from "cypress/api/concernsApi";

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

    it("Should validate adding a case", () => {
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
            .then(row => {
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
        caseManagementPage.addAnotherConcernForNonConcern();

        Logger.Log("Checking accessibility on adding concern page");
        cy.excuteAccessibilityTests();
        
        Logger.Log("Attempt to create an invalid concern");
        createConcernPage
            .addConcern()
            .hasValidationError("Select concern type")
            .hasValidationError("Select risk rating")
            .hasValidationError("Select means of referral");
            
        cy.waitForJavascript();

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .addConcern();

        // Logger.Log("Adding another concern during case creation");
        createConcernPage
        //     .addAnotherConcern()
        //     .withConcernType("Governance and compliance")
        //     .withSubConcernType("Governance and compliance: Compliance")
        //     .withRating("Amber-Green")
        //     .withMeansOfRefferal("Internal")
        //     .addConcern()
            .nextStep();


        Logger.Log("Check unpopulated risk to trust throws validation error");
        addConcernDetailsPage.nextStep()
            .hasValidationError("Select risk rating");

        createConcernPage.withRating("Red-Plus");
        cy.waitForJavascript();

        addConcernDetailsPage.nextStep();
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
            .hasValidationError("Case history must be 4300 characters or less");

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
          //  .hasConcerns("Governance and compliance: Compliance", ["Amber", "Green"])
            .hasConcerns("Financial: Deficit", ["Red", "Amber"])
            .hasTerritory("North and UTC - North East")
            .hasIssue("This is an issue")
            .hasCurrentStatus("This is the current status")
            .hasCaseAim("This is the case aim")
            .hasDeEscalationPoint("This is the de-escalation point")
            .hasNextSteps("This is the next steps")
            .hasCaseHistory("This is the case history");
            
            Logger.Log("Verify the means of referral is set");
            caseManagementPage.getCaseIDText().then((caseId) => {
                concernsApi.get(parseInt(caseId))
                    .then(response => {
                        expect(response[0].meansOfReferralId).to.eq(2);
                    });
            });
    });

    function closeCase(caseId: string) {
        Logger.Log("Closing case");
        CaseManagementPage.getCloseCaseBtn().click();

        CaseManagementPage.withRationaleForClosure("Closing non concerns case");
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
            .hasRationaleForClosure("Closing non concerns case")
            .hasNoCaseNarritiveFields();
    }
});