import trustOverviewPage from "cypress/pages/trustOverviewPage";
import { Logger } from "../../common/logger";
import createCaseSummary from "cypress/pages/createCase/createCaseSummary";
import selectCaseTypePage from "cypress/pages/createCase/selectCaseTypePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import caseApi from "cypress/api/caseApi";
import { CaseBuilder } from "cypress/api/caseBuilder";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import { PaginationComponent } from "cypress/pages/paginationComponent";

describe("User interactions via Find Trust route", () => {

    const createConcernPage = new CreateConcernPage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();
    const addDetailsPage = new AddDetailsPage();
	const paginationComponent = new PaginationComponent("active-cases-");

	describe("When we find a trust", () => 
	{
		beforeEach(() => {
			cy.login();
		});
	
		const trustName = "Accrington St Christopher's Church Of England High School";
	
		it("Should display details on the selected trust and create a case against the trust", () => {
			Logger.Log("User clicks on Create Case and should see Search Trusts");
			cy.get('[href="/trust"]').click();
			cy.get("#search").should("be.visible");
		
			Logger.Log("It should display an error when the user does not enter a trust");
			cy.get("#search").type(trustName.substring(0, 1) + "{enter}");
			cy.getById("errorSummary").should("contain.text", "A trust must be selected");
		
			Logger.Log("Getting too many search results should result in a warning");
			const warningMessage = "There are a large number of search results. Try a more specific search term.";
			cy.get("#search").type("sch");
			cy.getById("tooManyResultsWarning").should(
				"contain.text",
				warningMessage
			);
		
			Logger.Log("Search for a valid trust should find multiple results");
			cy.get("#search").clear().type("Manchester");
			cy.get("#search__listbox").children().should("have.length.gt", 1);
		
			Logger.Log("Selecting the trust should populate it in the search");
			cy.get("#search").clear().type(trustName);
			cy.get("#search__option--0").click();
			cy.get("#search").should(
				"have.value",
				`${trustName}, 10059220, 07728029 (Accrington)`
			);
		
			Logger.Log("Clicking continue should populate the trust page");
			cy.getById("continue").click();
			cy.url().should("include", "/trust");
		
			Logger.Log("Trust page should have title of the academy");
			cy.get(".govuk-heading-l").should(
				"contain.text",
				trustName
			);
			
			Logger.Log("Ensure that the trust information is populated");
			trustOverviewPage
				.trustTypeIsNotEmpty()
				.trustAddressIsNotEmpty()
				.trustAcademiesIsNotEmpty()
				.trustPupilCapacityIsNotEmpty()
				.trustPupilNumbersIsNotEmpty()
				.trustGroupIdIsNotEmpty()
				.trustUKPRNIsNotEmpty()
				.trustCompanyHouseNumberIsNotEmpty();
	
			Logger.Log("Checking accessibility on risk to trust");
			cy.excuteAccessibilityTests();
	
			Logger.Log("Create a case against the trust");
			trustOverviewPage.createCase();
	
			createCaseSummary
				.hasTrustSummaryDetails(trustName);
	
			selectCaseTypePage
				.withCaseType("Concerns")
				.continue();
	
			Logger.Log("Create a valid concern");
			createConcernPage
				.withConcernType("Financial")
				.withSubConcernType("Financial: Deficit")
				.withConcernRating("Red-Amber")
				.withMeansOfReferral("External")
				.addConcern();
	
			Logger.Log("Check Concern details are correctly populated");
			createCaseSummary
				.hasTrustSummaryDetails(trustName)
				.hasConcernType("Financial: Deficit")
				.hasConcernRiskRating("Red Amber");
	
			createConcernPage
				.nextStep();
			
			Logger.Log("Populate risk to trust");
			addDetailsPage
				.withRiskToTrust("Red-Plus")
				.nextStep();
	
			Logger.Log("Check Trust, concern and risk to trust details are correctly populated");
			createCaseSummary
				.hasTrustSummaryDetails(trustName)
				.hasConcernType("Financial: Deficit")
				.hasConcernRiskRating("Red Amber")
				.hasRiskToTrust("Red Plus");
	
			Logger.Log("Populate territory");
			addTerritoryPage
				.withTerritory("North and UTC - North East")
				.nextStep();
	
			Logger.Log("Check Trust, concern, risk to trust details and territory are correctly populated");
			createCaseSummary
				.hasTrustSummaryDetails(trustName)
				.hasConcernType("Financial: Deficit")
				.hasConcernRiskRating("Red Amber")
				.hasRiskToTrust("Red Plus")
				.hasTerritory("North and UTC - North East");
	
			Logger.Log("Add concern details with valid text limit");
			addConcernDetailsPage
				.withIssue("This is an issue")
				.createCase();
	
			Logger.Log("Verify case details");
			caseManagementPage
				.hasTrust(trustName)
				.hasRiskToTrust("Red Plus")
				.hasConcerns("Financial: Deficit", ["Red", "Amber"])
				.hasTerritory("North and UTC - North East")
				.hasIssue("This is an issue");
		});
	})

	describe("When the trust has many open cases", () =>
    {
		const trustUkPrn = "10060832";

        beforeEach(() =>
        {
            cy.login();

            // Ensure we have enough cases
            caseApi.getOpenCasesByTrust(trustUkPrn)
            .then((response) =>
            {
                const currentCases = response.paging.recordCount;
                const casesToCreate = 15 - currentCases;

                if (casesToCreate > 0)
                {
                    const interator = Array.from({ length: casesToCreate });

                    cy.wrap(interator).each(($el, index, $list) => {
                        cy.wait(10);

                        const request = CaseBuilder.buildOpenCase();
                        request.trustUkprn = trustUkPrn;

                        caseApi.post(request)
                            .then(() => {});
                    });

                    cy.reload();
                }
            });
        });

        it("Should display them in separate pages with 5 items per page and we should be able to move between them", () =>
        {
            let pageOneCases: Array<string> = [];
            let pageTwoCases: Array<string> = [];

			cy.visit(`/trust/${trustUkPrn}/overview`);

            caseworkTable
                .getCaseIds()
                .then((caseIds: Array<string>) =>
                {
                    pageOneCases = caseIds;

                    Logger.Log("Ensure we have 5 cases on page one")
                    expect(pageOneCases.length).to.eq(5);

                    Logger.Log("Moving to the second page using the direct link");
                    paginationComponent
						.goToPage("2")
						.isCurrentPage("2");

                    return caseworkTable.getCaseIds()
                })
                .then((caseIds: Array<string>) =>
                {
                    pageTwoCases = caseIds;

                    Logger.Log("Ensure we have 5 cases on page 2");
                    expect(pageTwoCases.length).to.equal(5);
                    
                    Logger.Log("Ensure that the cases on page one and two are different");
                    hasNoSimilarElements(pageOneCases, pageTwoCases);

                    Logger.Log("Move to the previous page, which is page 1")
                    paginationComponent
						.previous()
						.isCurrentPage("1");

                    return caseworkTable.getCaseIds()
                })
                .then((caseIds: Array<string>) =>
                {
                    Logger.Log("On moving to page one, we should get the exact same cases");
                    expect(caseIds).to.deep.equal(pageOneCases);

                    Logger.Log("Move to the next page, which is page 2");
                    paginationComponent
						.next()
						.isCurrentPage("2");

                    return caseworkTable.getCaseIds();
                })
                .then((caseIds: Array<string>) =>
                {
                    Logger.Log("On moving to page two, we should get the exact same cases");
                    expect(caseIds).to.deep.equal(pageTwoCases);
                });
        });
    });

    function hasNoSimilarElements(first: Array<string>, second: Array<string>)
    {
        var firstSet = new Set(first);

        const match = second.some(e => firstSet.has(e));

        expect(match).to.be.false;
    }
});
	
