import { Logger } from "../../../common/logger";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { EditFinancialPlanPage } from "../../../pages/caseActions/financialPlan/editFinancialPlanPage";
import { ViewFinancialPlanPage } from "../../../pages/caseActions/financialPlan/viewFinancialPlanPage";
import { CloseFinancialPlanPage } from "../../../pages/caseActions/financialPlan/closeFinancialPlanPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";

describe("User can add Financial Plan case action to an existing case", () => {
    let viewFinancialPlanPage = new ViewFinancialPlanPage();
    let editFinancialPlanPage = new EditFinancialPlanPage();
    let closeFinancialPlanPage = new CloseFinancialPlanPage();
    let now: Date;
    
    beforeEach(() => {
        cy.login();

        now = new Date();
        Logger.Log("Given a case");
        cy.basicCreateCase();
        addFinancialPlanToCase();
    });

    it("Should add a financial plan", () => 
    {
        checkFormValidation();

        Logger.Log("Configuring a valid financial plan");

        editFinancialPlanPage
            .withPlanRequestedDay("06")
            .withPlanRequestedMonth("07")
            .withPlanRequestedYear("2022")
            .withNotes("Notes!")
            .save();

        Logger.Log("Selecting Financial Plan from open actions");
		actionSummaryTable
			.getOpenAction("Financial Plan")
			.then(row =>
			{
				row.hasName("Financial Plan")
				row.hasStatus("In progress")
				row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

        Logger.Log("Checking Financial Plan values");

        viewFinancialPlanPage
            .hasDateOpened(toDisplayDate(now))
            .hasPlanRequestedDate("06 July 2022")
            .hasNotes("Notes!");
    });

    it("Should handle an empty form", () =>
    {
        editFinancialPlanPage.save();

        Logger.Log("Selecting Financial Plan from open actions");

		actionSummaryTable
			.getOpenAction("Financial Plan")
			.then(row =>
			{
				row.select();
			});

        viewFinancialPlanPage
            .hasDateOpened(toDisplayDate(now))
            .hasStatus("In progress")
            .hasPlanRequestedDate("Empty")
            .hasNotes("Empty");
    });

    it("Should edit an existing financial plan", () => 
    {
        Logger.Log("Configuring initial financial plan");

        editFinancialPlanPage
            .withPlanRequestedDay("06")
            .withPlanRequestedMonth("07")
            .withPlanRequestedYear("2022")
            .withNotes("Notes!")
            .save();

        Logger.Log("Selecting Financial Plan from open actions");

		actionSummaryTable
			.getOpenAction("Financial Plan")
			.then(row =>
			{
				row.select();
			});

        Logger.Log("Ensure values are displayed correctly");

        viewFinancialPlanPage.edit();

        editFinancialPlanPage
            .hasPlanRequestedDay("06")
            .hasPlanRequestedMonth("07")
            .hasPlanRequestedYear("2022")
            .hasNotes("Notes!");

        Logger.Log("Changing the financial plan");

        editFinancialPlanPage
            .withPlanRequestedDay("01")
            .withPlanRequestedMonth("02")
            .withPlanRequestedYear("2007")
            .withNotes("Editing notes")
            .save();

        Logger.Log("Selecting Financial Plan from open actions");

		actionSummaryTable
			.getOpenAction("Financial Plan")
			.then(row =>
			{
				row.select();
			});

        Logger.Log("Viewing edited Financial Plan values");

        viewFinancialPlanPage
            .hasStatus("In progress")
            .hasPlanRequestedDate("01 February 2007")
            .hasNotes("Editing notes");

        viewFinancialPlanPage.edit();

        checkFormValidation();
    });

    it("Should close a financial plan", () => 
    {
        Logger.Log("creating a financial plan");
        editFinancialPlanPage
            .withPlanRequestedDay("09")
            .withPlanRequestedMonth("02")
            .withPlanRequestedYear("2023")
            .withNotes("Initial Notes!")
            .save();

        Logger.Log("Selecting Financial Plan from open actions");
		actionSummaryTable
			.getOpenAction("Financial Plan")
			.then(row =>
			{
				row.select();
			});

        Logger.Log("Closing the financial plan");
        viewFinancialPlanPage
            .close();

        Logger.Log("Ensure user must select a reason for closing the financial plan");
        closeFinancialPlanPage
            .close()
            .hasValidationError("Please select a reason for closing the Financial Plan");

        Logger.Log("Ensure notes cannot exceed 2000 characters");
        closeFinancialPlanPage
            .withReasonForClosure("Abandoned")
            .withNotesExceedingLimit()
            .close()
            .hasValidationError("Notes must be 2000 characters or less")

        Logger.Log("Ensure a valid date must be entered");
        closeFinancialPlanPage
            .withReasonForClosure("Abandoned")
            .withPlanReceivedDay("32")
            .withPlanReceivedMonth("13")
            .withPlanReceivedYear("2020")
            .withNotes("Edited Notes!")
            .close()
            .hasValidationError("Viable plan 32-13-2020 is an invalid date")

        Logger.Log("Close a valid financial plan");
        closeFinancialPlanPage
            .withReasonForClosure("Abandoned")
            .withPlanReceivedDay("10")
            .withPlanReceivedMonth("02")
            .withPlanReceivedYear("2023")
            .withNotes("Edited Notes!")
            .close();

        Logger.Log("Selecting Financial Plan from close actions");
		actionSummaryTable
			.getClosedAction("Financial Plan")
			.then(row =>
			{
				row.hasName("Financial Plan");
				row.hasStatus("Abandoned");
				row.hasCreatedDate(toDisplayDate(now));
                row.hasClosedDate(toDisplayDate(now));
				row.select();
			});

        Logger.Log("Ensure values are displayed correctly");
        viewFinancialPlanPage
            .hasDateOpened(toDisplayDate(now))
            .hasDateClosed(toDisplayDate(now))
            .hasStatus("Abandoned")
            .hasPlanRequestedDate("09 February 2023")
            .hasPlanReceivedDate("10 February 2023")
            .hasNotes("Edited Notes!");
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
            .withNotes("Notes for validation")
            .clearPlanRequestedDate()
            .withPlanRequestedDay("06")
            .save()
            .hasValidationError("Plan requested 06-- is an invalid date");

        Logger.Log("Check fields were not cleared on error");

        editFinancialPlanPage
            .hasNotes("Notes for validation");

        Logger.Log("Invalid plan requested date");

        editFinancialPlanPage
            .clearPlanRequestedDate()
            .withPlanRequestedDay("06")
            .withPlanRequestedMonth("22")
            .withPlanRequestedYear("22")
            .save()
            .hasValidationError("Plan requested 06-22-22 is an invalid date");

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
