import { Logger } from "../../../common/logger";

export class ViewNtiWarningLetterPage {
    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`nti-status`).should("contain.text", value);

        return this;
    }

    public hasDateSent(value: string): this {
        Logger.Log(`Has day sent ${value}`);

        cy.getByTestId(`nti-date-sent`).should("contain.text", value);

        return this;
    }

    public hasReason(value: string): this {
        Logger.Log(`Has reason ${value}`);

        cy.getByTestId(`nti-reasons`).should("contain.text", value);

        return this;
    }

    public hasReasonCount(value: number): this
    {
        Logger.Log(`Has reason count ${value}`);

        cy.getByTestId("nti-reasons").children().should("have.length", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getByTestId(`nti-notes`).should("contain.text", value);

        return this;
    }

    public hasCondition(value: string): this {
        Logger.Log(`Has condition ${value}`);

        cy.getByTestId(`nti-conditions`).should("contain.text", value);

        return this;
    }

    public hasConditionCount(value: number): this
    {
        Logger.Log(`Has condition count ${value}`);

        cy.getByTestId(`nti-conditions`).children().should("have.length", value);

        return this;
    }

    public edit(): this
    {
        Logger.Log("Editing the NTI warning letter");

        this.getEdit().click();

        return this;
    }

    public close(): this {
        Logger.Log("Closing NTI warning letter");

        this.getClose().click();

        return this;
    }

    public cannotEdit()
    {
        Logger.Log("Ensure we cannot edit");

        this.getEdit().should("not.exist");

        return this;
    }

    public cannotClose()
    {
        Logger.Log("Ensure we cannot close");

        this.getClose().should("not.exist");

        return this;
    }

    private getEdit()
    {
        return cy.getById("edit-nti-wl-button");
    }

    private getClose()
    {
        return cy.getByTestId("close-nti-wl-button");
    }
}