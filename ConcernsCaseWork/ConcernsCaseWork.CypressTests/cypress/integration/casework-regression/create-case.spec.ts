import { Logger } from "cypress/common/logger";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import CaseManagementPage from "cypress/pages/createCase/managementPage";

describe("Creating a case", () =>
{
	const createCasePage = new CreateCasePage();
    const createConcernPage = new CreateConcernPage();
    const addDetailsPage = new AddDetailsPage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    const caseManagementPage = new CaseManagementPage();
    
	beforeEach(() => {
		cy.login();
	});

    it("Should validate unpopulated concern values", () =>
    {
        Logger.Log("Create a case");
        createCasePage
            .clickCreateCaseButton()
            .withTrustName("Ashton West End Primary Academy")
            .clickFirstOption()
            .clickConfirmOptionButton();


        //TODO: Check that trust field is correctly populated

        Logger.Log("Attempt to create an invalid concern");
        createConcernPage
            .clickAddConcernButton()
            .hasValidationError("Select concern type")
            .hasValidationError("Select risk rating")
            .hasValidationError("Select means of referral");

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .clickAddConcernButton();

    });

    it("Should create a valid concern", () =>
    {
        Logger.Log("Create a case");
        createCasePage
            .clickCreateCaseButton()
            .withTrustName("Ashton West End Primary Academy")
            .clickFirstOption()
            .clickConfirmOptionButton();

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .clickAddConcernButton();


        //TODO: Check that trust and concerns field is correctly populated

    });

    it("Should validate adding risk to trust", () =>
    {
        Logger.Log("Create a case");
        createCasePage
            .clickCreateCaseButton()
            .withTrustName("Ashton West End Primary Academy")
            .clickFirstOption()
            .clickConfirmOptionButton();

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .clickAddConcernButton()
            .clickNextStepButton();

        Logger.Log("Check unpopulated risk to trust throws validation error");
        addDetailsPage
            .clickNextStepButton()
            .hasValidationError("Select risk rating");
    });

    it("Should validate adding territory", () =>
    {
        Logger.Log("Create a case");
        createCasePage
            .clickCreateCaseButton()
            .withTrustName("Ashton West End Primary Academy")
            .clickFirstOption()
            .clickConfirmOptionButton();

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .clickAddConcernButton()
            .clickNextStepButton();

        //TODO: Check the risk to trust filed is correctly populated

        Logger.Log("Populate risk to trust");
        addDetailsPage
            .withRating("Red-Plus")
            .clickNextStepButton();

        Logger.Log("Check unpopulated territory throws validation error");
        addTerritoryPage
            .clickNextStepButton()
            .hasValidationError("An SFSO Territory must be selected");

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory("North and UTC - North East")
            .clickNextStepButton();
    });

    it("Should add concern details", () =>
    {
        Logger.Log("Create a case");
        createCasePage
            .clickCreateCaseButton()
            .withTrustName("Ashton West End Primary Academy")
            .clickFirstOption()
            .clickConfirmOptionButton();

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .clickAddConcernButton()
            .clickNextStepButton();

        //TODO: Check the risk the territory field is correctly populated

        Logger.Log("Populate risk to trust");
        addDetailsPage
            .withRating("Red-Plus")
            .clickNextStepButton();

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory("North and UTC - North East")
            .clickNextStepButton();

        Logger.Log("Validate unpopulated concern details");
        addConcernDetailsPage
            .withExceedingTextLimit("issue", 2001)
            .withExceedingTextLimit("current-status", 4001)
            .withExceedingTextLimit("case-aim", 1001)
            .withExceedingTextLimit("de-escalation-point", 1001)
            .withExceedingTextLimit("next-steps", 4001)
            .withExceedingTextLimit("case-history", 4300)
            .clickCreateCaseButton()
            .hasValidationError("Issue must be 2000 characters or less")
            .hasValidationError("Current status must be 4000 characters or less")
            .hasValidationError("Next steps must be 4000 characters or less")
            .hasValidationError("De-escalation point must be 1000 characters or less")
            .hasValidationError("Case aim must be 1000 characters or less")
            .hasValidationError("Case history must be 4300 characters or less");

            Logger.Log("Add concern details with valid text limit");
            addConcernDetailsPage
                .withIssue("This is an issue")
                .withCurrentStatus("This is the current status")
                .withCaseAim("This is the case aim")
                .withDeEscalationPoint("This is the de-escalation point")
                .withNextSteps("This is the next steps")
                .withCaseHistory("This is the case history")
                .clickCreateCaseButton();

            
            Logger.Log("Verify case details");
            caseManagementPage
                .hasIssue("This is an issue")
                .hasCurrentStatus("This is the current status")
                .hasCaseAim("This is the case aim")
                .hasDeEscalationPoint("This is the de-escalation point")
                .hasNextSteps("This is the next steps")
                .hasCaseHistory("This is the case history");
    });
});