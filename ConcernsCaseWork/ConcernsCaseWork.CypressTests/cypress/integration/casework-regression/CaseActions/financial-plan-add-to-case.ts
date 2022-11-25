import { isSymbol } from "cypress/types/lodash";
import { Logger } from "../../../common/logger";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { FinancialPlanPage } from "../../../pages/caseActions/financialPlanPage";
import CaseManagementPage from "../../../pages/caseMangementPage";

describe("User can add Financial Plan case action to an existing case", () => {
    let financialPlanPage = new FinancialPlanPage();
    
    beforeEach(() => {
        cy.login();

        Logger.Log("Given a case");
        cy.basicCreateCase();
        addFinancialPlanToCase();
    });

    afterEach(() => {
        cy.storeSessionData();
    });

    it("Should add a financial plan", () => 
    {
        validateDates();

        Logger.Log("Configuring a valid financial plan");

        financialPlanPage
            .withStatus("Awaiting Plan")
            .withPlanRequestedDay("06")
            .withPlanRequestedMonth("07")
            .withPlanRequestedYear("2022")
            .withPlanReceivedDay("22")
            .withPlanReceivedMonth("10")
            .withPlanReceivedYear("2022")
            .withNotes("Notes!")
            .save();

        Logger.Log("Selecting Financial Plan from open actions");

        cy.get("#open-case-actions td")
            .should("contain.text", "Financial Plan")
            .eq(-3)
            .children("a")
            .click();

        Logger.Log("Checking Financial Plan values");

        financialPlanPage
            .hasStatus("Awaiting plan")
            .hasPlanRequestedDate("06-07-2022")
            .hasPlanReceivedDate("22-10-2022")
            .hasNotes("Notes!");
    });

    it("Should handle an empty form", () =>
    {
        financialPlanPage.save();

        Logger.Log("Selecting Financial Plan from open actions");

        cy.get("#open-case-actions td")
            .should("contain.text", "Financial Plan")
            .eq(-3)
            .children("a")
            .click();

        financialPlanPage
            .hasStatus("In progress")
            .hasPlanRequestedDate("Empty")
            .hasPlanReceivedDate("Empty")
            .hasNotes("Empty");
    });

    it("Should edit an existing financial plan", () => 
    {
        Logger.Log("Configuring initial financial plan");

        financialPlanPage
            .withStatus("Awaiting Plan")
            .withPlanRequestedDay("06")
            .withPlanRequestedMonth("07")
            .withPlanRequestedYear("2022")
            .withPlanReceivedDay("22")
            .withPlanReceivedMonth("10")
            .withPlanReceivedYear("2022")
            .withNotes("Notes!")
            .save();

        Logger.Log("Selecting Financial Plan from open actions");

        cy.get("#open-case-actions td")
            .should("contain.text", "Financial Plan")
            .eq(-3)
            .children("a")
            .click();

        Logger.Log("Changing the financial plan");

        financialPlanPage
            .edit()
            .withStatus("Return To Trust")
            .withPlanRequestedDay("01")
            .withPlanRequestedMonth("02")
            .withPlanRequestedYear("2007")
            .withPlanReceivedDay("05")
            .withPlanReceivedMonth("07")
            .withPlanReceivedYear("2008")
            .withNotes("Editing notes")
            .save();

        Logger.Log("Selecting Financial Plan from open actions");

        cy.get("#open-case-actions td")
            .should("contain.text", "Financial Plan")
            .eq(-3)
            .children("a")
            .click();

        Logger.Log("Checking edited Financial Plan values");

        financialPlanPage
            .hasStatus("Return to trust for further work")
            .hasPlanRequestedDate("01-02-2007")
            .hasPlanReceivedDate("05-07-2008")
            .hasNotes("Editing notes");

        financialPlanPage.edit();

        validateDates();
    });

    it("Should only let one financial plan be created per case", () => 
    {
        Logger.Log("Configuring first financial plan");

        financialPlanPage.save();

        Logger.Log("Try to add second financial plan to case");
        addFinancialPlanToCase();

        cy.getByTestId("error-text")
            .should("contain.text", "There is already an open Financial Plan action linked to this case. Please resolve that before opening another one.");
    });

    function validateDates()
    {
        Logger.Log("Incomplete plan requested date");

        financialPlanPage
            .clearPlanRequestedDate()
            .withPlanRequestedDay("06")
            .save()
            .hasValidationError("Plan requested 06-- is an invalid date");

        Logger.Log("Invalid plan requested date");

        financialPlanPage
            .clearPlanRequestedDate()
            .withPlanRequestedDay("06")
            .withPlanRequestedMonth("22")
            .withPlanRequestedYear("22")
            .save()
            .hasValidationError("Plan requested 06-22-22 is an invalid date");

        Logger.Log("Incomplete plan received date");

        financialPlanPage
            .clearPlanReceivedDate()
            .withPlanReceivedDay("08")
            .save()
            .hasValidationError("Viable plan 08-- is an invalid date");

        Logger.Log("Invalid plan received date");

        financialPlanPage
            .clearPlanReceivedDate()
            .withPlanReceivedDay("08")
            .withPlanReceivedMonth("33")
            .withPlanReceivedYear("33")
            .save()
            .hasValidationError("Viable plan 08-33-33 is an invalid date");
    }

    function addFinancialPlanToCase()
    {
        Logger.Log("Has option to add Financial Plan Case Action to a case");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('FinancialPlan')
        AddToCasePage.getCaseActionRadio('FinancialPlan').siblings().should('contain.text', AddToCasePage.actionOptions[2]);
        AddToCasePage.getAddToCaseBtn().click();
    }
});

