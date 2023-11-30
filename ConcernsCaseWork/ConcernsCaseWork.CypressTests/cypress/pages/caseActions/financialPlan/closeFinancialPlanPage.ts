import { Logger } from "../../../common/logger";

export class CloseFinancialPlanPage
{
    public withReasonForClosure(reason: string): this
    {
        Logger.log(`With reason for closure ${reason}`);

        cy.getByTestId(reason).click();

        return this;
    }

    public withPlanReceivedDay(day: string): this
    {
        Logger.log(`With plan received day ${day}`);

        cy.getByTestId("dtr-day-date-viable-plan-received").clear().type(day);

        return this;
    }

    public withPlanReceivedMonth(month: string): this
    {
        Logger.log(`With plan received month ${month}`);

        cy.getByTestId("dtr-month-date-viable-plan-received").clear().type(month);

        return this;
    }

    public withPlanReceivedYear(year: string): this
    {
        Logger.log(`With plan received year ${year}`);

        cy.getByTestId("dtr-year-date-viable-plan-received").clear().type(year);

        return this;
    }

    public withNotes(notes: string): this
    {
        Logger.log(`With notes ${notes}`);

        cy.getById("financial-plan-notes").clear().type(notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
		Logger.log("With notes exceeding the limit");

		cy.getById("financial-plan-notes").clear().invoke("val", "x".repeat(2001));

		return this;
	}

    public clearPlanReceivedDate(): this
    {
        Logger.log("Clearing plan received date");

        cy.getByTestId("dtr-day-date-viable-plan-received").clear();
        cy.getByTestId("dtr-month-date-viable-plan-received").clear();
        cy.getByTestId("dtr-year-date-viable-plan-received").clear();

        return this;
    }

    public close()
    {
        Logger.log("Confirming the closure of the financial plan");

        cy.getById("close-financial-plan-button").click();

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);
        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

}