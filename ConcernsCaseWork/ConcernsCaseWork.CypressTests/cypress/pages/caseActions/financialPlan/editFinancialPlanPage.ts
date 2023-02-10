import { Logger } from "../../../common/logger";

export class EditFinancialPlanPage 
{
    public hasHeadingText(heading: string): this
    {
        Logger.Log(`Has heading ${heading}`);

        cy.get('h1[class="govuk-heading-l"]').should("contain.text", heading);

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



    public withNotes(notes: string): this
    {
        Logger.Log(`With notes ${notes}`);

        cy.getById("financial-plan-notes").clear().type(notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
		cy.task("log", `With Notes exceeding limit`);

		cy.getById("financial-plan-notes").clear().invoke("val", "x".repeat(2001));

		return this;
	}

    public save(): this
    {
        Logger.Log("Save financial plan");

        cy.getById("add-financial-plan-button").click();

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
        Logger.Log(`With entered status ${status}`);

        var selector = `status-${status.split(" ").join("")}`;

        cy.getByTestId(selector).should("be.checked");

        return this;
    }

    public hasPlanRequestedDay(day: string): this
    {
        Logger.Log(`Has entered plan requested day ${day}`);

        cy.getById("dtr-day-plan-requested").should("have.value", day);

        return this;
    }

    public hasPlanRequestedMonth(month: string): this
    {
        Logger.Log(`Has entered plan requested month ${month}`);

        cy.getById("dtr-month-plan-requested").should("have.value", month);

        return this;
    }

    public hasPlanRequestedYear(year: string): this
    {
        Logger.Log(`Has entered plan requested year ${year}`);

        cy.getById("dtr-year-plan-requested").should("have.value", year);

        return this;
    }

    public hasPlanReceivedDay(day: string): this
    {
        Logger.Log(`Has entered plan received day ${day}`);

        cy.getById("dtr-day-viable-plan").should("have.value", day);

        return this;
    }

    public hasPlanReceivedMonth(month: string): this
    {
        Logger.Log(`Has entered plan received month ${month}`);

        cy.getById("dtr-month-viable-plan").should("have.value", month);

        return this;
    }

    public hasPlanReceivedYear(year: string): this
    {
        Logger.Log(`Has entered plan received year ${year}`);

        cy.getById("dtr-year-viable-plan").should("have.value", year);

        return this;
    }

    public hasNotes(notes: string): this
    {
        Logger.Log(`With entered notes ${notes}`);

        cy.getById("financial-plan-notes").should("contain.text", notes);

        return this;
    }
}