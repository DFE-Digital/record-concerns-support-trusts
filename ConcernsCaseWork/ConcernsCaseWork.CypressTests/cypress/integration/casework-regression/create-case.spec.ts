import { Logger } from "cypress/common/logger";
import caseMangementPage from "cypress/pages/caseMangementPage";
import editCaseManagementPage from "cypress/pages/editCaseManagementPage";
import homePage from "cypress/pages/homePage";

describe("Creating a case", () =>
{
	beforeEach(() => {
		cy.login();
	});

    it("Should create a case with the values provided", () =>
    {
        homePage.createCase();
        cy.randomSelectTrust();
        cy.get("#search__option--0").click();
		cy.getById("continue").click();

        editCaseManagementPage.hasNoConcernType("Safeguarding");
        
		cy.selectConcernType();
		cy.selectRiskToTrust();
		cy.selectTerritory();

        Logger.Log("Validating concern details");

        caseMangementPage
            .createCase()
            .hasValidationError("Issue is required");

        editCaseManagementPage.withIssue("This is an issue");
        cy.waitForJavascript();

        editCaseManagementPage.withCaseHistoryExceedingLimit();

        caseMangementPage
            .createCase()
            .hasValidationError("Case history must be 4300 characters or less");

        editCaseManagementPage
            .withCaseHistory("My case history")
        cy.waitForJavascript();

        Logger.Log("Verifying case history");

        caseMangementPage
            .createCase()
            .showAllConcernDetails()
            .hasCaseHistory("My case history");

        Logger.Log("Editing case history");

        caseMangementPage.editCaseHistory();

        editCaseManagementPage.hasCaseHistory("My case history");

        editCaseManagementPage
            .withCaseHistoryExceedingLimit()
            .save()
            .hasValidationError("Case history must be 4300 characters or less")
            .withCaseHistory("My new case history");
        cy.waitForJavascript();

        editCaseManagementPage.save();

        caseMangementPage.hasCaseHistory("My new case history");
    });
});