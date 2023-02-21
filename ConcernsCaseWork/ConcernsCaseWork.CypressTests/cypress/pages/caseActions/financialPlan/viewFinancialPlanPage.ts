import { Logger } from "../../../common/logger";

export class ViewFinancialPlanPage
{
    public hasDateOpened(value: string): this {
        Logger.Log(`Has date opened ${value}`);

        cy.getByTestId("date-opened").should("contain.text", value);

        return this;
    }

    public hasDateClosed(value: string): this {
        Logger.Log(`Has date closed ${value}`);

        cy.getByTestId("date-closed").should("contain.text", value);

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

    public edit(): this
    {
        Logger.Log("Edit financial plan");

        this.getEdit().click();

        return this;
    }

    public canEdit()
    {
        Logger.Log("Can edit financial plan");

        this.getEdit();

        return this;
    }

    public cannotEdit()
    {
        Logger.Log("Cannot edit financial plan");

        this.getEdit().should("not.exist");

        return this;
    }

    public close(): this
    {
        Logger.Log("Closing financial plan");

        this.getClose().click();

        return this;
    }

    public canClose(): this
    {
        Logger.Log("Can close financial plan");

        this.getClose();

        return this;
    }

    public cannotClose(): this
    {
        Logger.Log("Cannot close financial plan");

        this.getClose().should("not.exist");

        return this;
    }

    private getEdit()
    {
        return cy.getById("edit-financialplan-button");
    }

    private getClose() {
        return cy.getByTestId("close-financialplan-button");
    }
}