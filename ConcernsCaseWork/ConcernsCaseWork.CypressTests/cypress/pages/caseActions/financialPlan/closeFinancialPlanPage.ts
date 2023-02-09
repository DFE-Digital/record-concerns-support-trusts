import { Logger } from "../../../common/logger";

export class CloseFinancialPlanPage
{
    public withReasonForClosure(reason: string): this
    {
        Logger.Log(`With reason for closure ${reason}`);

        const id = reason.split(" ").join("");

        cy.getByTestId(id).click();

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

    public withNotes(notes: string): this
    {
        Logger.Log(`With notes ${notes}`);

        cy.getById("financial-plan-notes").clear().type(notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
		Logger.Log("With notes exceeding the limit");

		cy.getById("financial-plan-notes").clear().invoke("val", "x".repeat(2001));

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

    public close()
    {
        Logger.Log("Confirming the closure of the financial plan");

        cy.getById("add-financial-plan-button").click();

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);
        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

}