import { Logger } from "../../common/logger";

export class FinancialPlanPage 
{
    public hasHeadingText(heading: string): this
    {
        Logger.Log(`Has heading ${heading}`);

        cy.get('h1[class="govuk-heading-l"]').should("contain.text", heading);

        return this;
    }

    public withStatus(status: string): this
    {
        Logger.Log(`With status ${status}`);

        var selector = `status-${status.split(" ").join("")}`;

        cy.getByTestId(selector).click();

        return this;
    }

    public withPlanRequestedDay(day: string): this
    {
        Logger.Log(`With plan requested day ${day}`);

        cy.getById("dtr-day-plan-requested").clear().type(day);

        return this;
    }

    public withPlanRequestedMonth(month: string): this
    {
        Logger.Log(`With plan requested month ${month}`);

        cy.getById("dtr-month-plan-requested").clear().type(month);

        return this;
    }

    public withPlanRequestedYear(year: string): this
    {
        Logger.Log(`With plan requested year ${year}`);

        cy.getById("dtr-year-plan-requested").clear().type(year);

        return this;
    }

    public clearPlanRequestedDate(): this
    {
        Logger.Log("Clearing plan requested date");

        cy.getById("dtr-day-plan-requested").clear();
        cy.getById("dtr-month-plan-requested").clear();
        cy.getById("dtr-year-plan-requested").clear();

        return this;
    }

    public withPlanReceivedDay(day: string): this
    {
        Logger.Log(`With plan received day ${day}`);

        cy.getById("dtr-day-viable-plan").clear().type(day);

        return this;
    }

    public withPlanReceivedMonth(month: string): this
    {
        Logger.Log(`With plan received month ${month}`);

        cy.getById("dtr-month-viable-plan").clear().type(month);

        return this;
    }

    public withPlanReceivedYear(year: string): this
    {
        Logger.Log(`With plan received year ${year}`);

        cy.getById("dtr-year-viable-plan").clear().type(year);

        return this;
    }

    public clearPlanReceivedDate(): this
    {
        Logger.Log("Clearing plan received date");

        cy.getById("dtr-day-viable-plan").clear();
        cy.getById("dtr-month-viable-plan").clear();
        cy.getById("dtr-year-viable-plan").clear();

        return this;
    }

    public withNotes(notes: string): this
    {
        Logger.Log(`With notes ${notes}`);

        cy.getById("financial-plan-notes").clear().type(notes);

        return this;
    }

    public save(): this
    {
        Logger.Log("Save financial plan");

        cy.getById("add-financial-plan-button").click();

        return this;
    }

    public edit(): this
    {
        Logger.Log("Edit financial plan");

        cy.getById("edit-financialplan-button").click();

        return this;
    }

    public hasValidationError(error: string): this
    {
        Logger.Log(`Has validation error ${error}`);

        cy.getById("errorSummary").should(
			"contain.text",
			error
		);

        return this;
    }

    public hasStatus(status: string): this
    {
        Logger.Log(`Has status ${status}`);

        cy.getByTestId("status-text").should("contain.text", status);

        return this;
    }

    public hasPlanRequestedDate(date: string): this
    {
        Logger.Log(`Has plan requested date ${date}`);

        cy.getByTestId("date-plan-requested-text").should("contain.text", date);

        return this;
    }

    public hasPlanReceivedDate(date: string): this
    {
        Logger.Log(`Has plan received date ${date}`);

        cy.getByTestId("date-plan-received-text").should("contain.text", date);

        return this;
    }

    public hasNotes(notes: string): this
    {
        Logger.Log(`Has notes ${notes}`);

        cy.getByTestId("notes-text");

        return this;
    }
}