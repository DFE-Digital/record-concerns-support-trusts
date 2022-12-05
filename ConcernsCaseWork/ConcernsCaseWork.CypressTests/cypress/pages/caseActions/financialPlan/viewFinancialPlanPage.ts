import { Logger } from "../../../common/logger";

export class ViewFinancialPlanPage
{
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

    public edit(): this
    {
        Logger.Log("Edit financial plan");

        cy.getById("edit-financialplan-button").click();

        return this;
    }
}