import { Logger } from "cypress/common/logger";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";

describe("Creating a case", () =>
{
	const createCasePage = new CreateCasePage();
    const createConcernPage = new CreateConcernPage();
    const addDetailsPage = new AddDetailsPage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    
	beforeEach(() => {
		cy.login();
	});

    it("Should validate adding a case", () =>
    {
        Logger.Log("Create a case");
        createCasePage
            .createCase()
            .withTrustName("Ashton West End Primary Academy")
            .selectOption()
            .confirmOption();

        Logger.Log("Attempt to create an invalid concern");
        createConcernPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .addConcern()
            .hasValidationError("Select concern type")
            .hasValidationError("Select risk rating")
            .hasValidationError("Select means of referral");

        cy.waitForJavascript();

        Logger.Log("Attempt to create a concern without a concern type");
        createConcernPage
            .withConcernType("Financial")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .addConcern()
            .hasValidationError("Select concern sub type");

        cy.waitForJavascript();

        Logger.Log("Create a valid concern");
        createConcernPage
            .withConcernType("Financial")
            .withSubConcernType("Financial: Deficit")
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .addConcern();

        Logger.Log("Check Concern details are correctly populated");
        createConcernPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .hasConcernType("Financial: Deficit")
            .nextStep();

        Logger.Log("Check unpopulated risk to trust throws validation error");
        addDetailsPage
            .nextStep()
            .hasValidationError("Select risk rating");
        
        cy.waitForJavascript();
        
        Logger.Log("Populate risk to trust");
        addDetailsPage
            .withRating("Red-Plus")
            .nextStep();

        Logger.Log("Check Trust, concern and risk to trust details are correctly populated");
        addTerritoryPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .hasConcernType("Financial: Deficit")
            .hasRiskToTrust("Red Plus")

        Logger.Log("Check unpopulated territory throws validation error");
        addTerritoryPage
            .nextStep()
            .hasValidationError("An SFSO Territory must be selected");

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory("North and UTC - North East")
            .nextStep();

        Logger.Log("Check Trust, concern, risk to trust details and territory are correctly populated");
        addConcernDetailsPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .hasConcernType("Financial: Deficit")
            .hasRiskToTrust("Red Plus")
            .hasTerritory("North and UTC - North East");


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
            .hasConcerns("Financial: Deficit")
            .hasTerritory("North and UTC - North East")
            .hasIssue("This is an issue")
            .hasCurrentStatus("This is the current status")
            .hasCaseAim("This is the case aim")
            .hasDeEscalationPoint("This is the de-escalation point")
            .hasNextSteps("This is the next steps")
            .hasCaseHistory("This is the case history");
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
            .withRating("Red-Amber")
            .withMeansOfRefferal("External")
            .addConcern();

        Logger.Log("Check Concern details are correctly populated");
        createConcernPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .hasConcernType("Financial: Deficit")
            .nextStep();

        Logger.Log("Populate risk to trust");
        addDetailsPage
            .withRating("Red-Plus")
            .nextStep();

        Logger.Log("Check Trust, concern and risk to trust details are correctly populated");
        addTerritoryPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .hasConcernType("Financial: Deficit")
            .hasRiskToTrust("Red Plus")

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory("North and UTC - North East")
            .nextStep();

        Logger.Log("Check Trust, concern, risk to trust details and territory are correctly populated");
        addConcernDetailsPage
            .hasTrustSummaryDetails("Ashton West End Primary Academy")
            .hasConcernType("Financial: Deficit")
            .hasRiskToTrust("Red Plus")
            .hasTerritory("North and UTC - North East");

        Logger.Log("Add concern details with valid text limit");
        addConcernDetailsPage
            .withIssue("This is an issue")
            .createCase();

        Logger.Log("Verify case details");
        caseManagementPage
            .hasTrust("Ashton West End Primary Academy")
            .hasRiskToTrust("Red Plus")
            .hasConcerns("Financial: Deficit")
            .hasTerritory("North and UTC - North East")
            .hasIssue("This is an issue")
            .hasEmptyCurrentStatus()
            .hasEmptyCaseAim()
            .hasEmptyDeEscalationPoint()
            .hasEmptyNextSteps()
            .hasEmptyCaseHistory();
    });

    const searchTerm =
		"Accrington St Christopher's Church Of England High School";
		let term = ""
	const searchTermForSchool = "school";

    it("User searches for a valid Trust and selects it", () => {

        createCasePage
            .createCase()
            .withTrustName(searchTerm)
            .selectOption()
            .confirmOption();

		Logger.Log("Should display the Concern details of the specified Trust");
		createConcernPage
            .hasTrustSummaryDetails(searchTerm)
            .cancel();
	});

    it('Should display an error if no trust is selected', () => {
		createCasePage
            .createCase()
            .withTrustName("A")
            .confirmOption()
            .hasValidationError("Select a trust");
    });

	it('Should display a warning if too many results', () => {
		createCasePage
            .createCase()
            .withTrustName(searchTermForSchool)
            .shouldNotHaveVisibleLoader()
            .hasTooManyResultsWarning("There are a large number of search results. Try a more specific search term.");
    });
    
});