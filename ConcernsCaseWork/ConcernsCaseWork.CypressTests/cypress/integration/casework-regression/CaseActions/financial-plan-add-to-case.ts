import { Logger } from "../../../common/logger";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { EditFinancialPlanPage } from "../../../pages/caseActions/financialPlan/editFinancialPlanPage";
import { ViewFinancialPlanPage } from "../../../pages/caseActions/financialPlan/viewFinancialPlanPage";
import CaseManagementPage from "../../../pages/caseMangementPage";

describe("User can add Financial Plan case action to an existing case", () => {
    let viewFinancialPlanPage = new ViewFinancialPlanPage();
    let editFinancialPlanPage = new EditFinancialPlanPage();
    
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
        checkFormValidation();

        Logger.Log("Configuring a valid financial plan");

        editFinancialPlanPage
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

        viewFinancialPlanPage
            .hasStatus("Awaiting plan")
            .hasPlanRequestedDate("06 July 2022")
            .hasPlanReceivedDate("22 October 2022")
            .hasNotes("Notes!");
    });

    it("Should handle an empty form", () =>
    {
        editFinancialPlanPage.save();

        Logger.Log("Selecting Financial Plan from open actions");

        cy.get("#open-case-actions td")
            .should("contain.text", "Financial Plan")
            .eq(-3)
            .children("a")
            .click();

        viewFinancialPlanPage
            .hasStatus("In progress")
            .hasPlanRequestedDate("Empty")
            .hasPlanReceivedDate("Empty")
            .hasNotes("Empty");
    });

    it("Should edit an existing financial plan", () => 
    {
        Logger.Log("Configuring initial financial plan");

        editFinancialPlanPage
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

        Logger.Log("Ensure values are displayed correctly");

        viewFinancialPlanPage.edit();

        editFinancialPlanPage
            .hasStatus("Awaiting Plan")
            .hasPlanRequestedDay("06")
            .hasPlanRequestedMonth("07")
            .hasPlanRequestedYear("2022")
            .hasPlanReceivedDay("22")
            .hasPlanReceivedMonth("10")
            .hasPlanReceivedYear("2022")
            .hasNotes("Notes!");

        Logger.Log("Changing the financial plan");

        editFinancialPlanPage
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

        Logger.Log("Viewing edited Financial Plan values");

        viewFinancialPlanPage
            .hasStatus("Return to trust for further work")
            .hasPlanRequestedDate("01 February 2007")
            .hasPlanReceivedDate("05 July 2008")
            .hasNotes("Editing notes");

        viewFinancialPlanPage.edit();

        checkFormValidation();
    });

    it("Should only let one financial plan be created per case", () => 
    {
        Logger.Log("Configuring first financial plan");

        editFinancialPlanPage.save();

        Logger.Log("Try to add second financial plan to case");
        addFinancialPlanToCase();

        cy.getByTestId("error-text")
            .should("contain.text", "There is already an open Financial Plan action linked to this case. Please resolve that before opening another one.");
    });

    function checkFormValidation()
    {
        Logger.Log("Incomplete plan requested date");

        editFinancialPlanPage
            .withStatus("Return To Trust")
            .withNotes("Notes for validation")
            .clearPlanRequestedDate()
            .withPlanRequestedDay("06")
            .save()
            .hasValidationError("Plan requested 06-- is an invalid date");

        Logger.Log("Check fields were not cleared on error");

        editFinancialPlanPage
            .hasStatus("Return To Trust")
            .hasNotes("Notes for validation");

        Logger.Log("Invalid plan requested date");

        editFinancialPlanPage
            .clearPlanRequestedDate()
            .withPlanRequestedDay("06")
            .withPlanRequestedMonth("22")
            .withPlanRequestedYear("22")
            .save()
            .hasValidationError("Plan requested 06-22-22 is an invalid date");

        Logger.Log("Incomplete plan received date");

        editFinancialPlanPage
            .clearPlanReceivedDate()
            .withPlanReceivedDay("08")
            .save()
            .hasValidationError("Viable plan 08-- is an invalid date");

        Logger.Log("Invalid plan received date");

        editFinancialPlanPage
            .clearPlanReceivedDate()
            .withPlanReceivedDay("08")
            .withPlanReceivedMonth("33")
            .withPlanReceivedYear("33")
            .save()
            .hasValidationError("Viable plan 08-33-33 is an invalid date");

        Logger.Log("Notes exceeding character limit");

        editFinancialPlanPage
                .withNotesExceedingLimit()
                .save()
                .hasValidationError("Notes must be 2000 characters or less");
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
