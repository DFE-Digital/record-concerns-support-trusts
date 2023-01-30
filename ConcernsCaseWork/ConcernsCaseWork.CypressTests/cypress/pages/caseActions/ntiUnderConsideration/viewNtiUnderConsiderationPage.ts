import { Logger } from "../../../common/logger";

export class ViewNtiUnderConsiderationPage
{
    public hasReason(value: string): this {
        Logger.Log(`Has reason ${value}`);

        cy.getByTestId(`nti-reasons`).should("contain.text", value);

        return this;
    }

    public hasReasonCount(value: number): this {
        Logger.Log(`Has reason count ${value}`);

        cy.getByTestId(`nti-reasons`).children().should("have.length", value);

        return this;
    }

    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`nti-status`).should("contain.text", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getByTestId(`nti-notes`).should("contain.text", value);

        return this;
    }

    public edit(): this
    {
        Logger.Log("Editing NTI under consideration");

        this.getEdit().click();

        return this;
    }

    public canEdit(): this
    {
        Logger.Log("Can edit");

        this.getEdit();

        return this;
    }

    public cannotEdit(): this
    {
        Logger.Log("Cannot edit");

        this.getEdit().should("not.exist");

        return this;
    }

    public close(): this
    {
        Logger.Log("Closing NTI under consideration");

        this.getClose().click();

        return this;
    }

    public canClose(): this
    {
        Logger.Log("Can close");

        this.getClose();

        return this;
    }

    public cannotClose(): this
    {
        Logger.Log("Cannot close");

        this.getClose().should("not.exist");

        return this;
    }

    private getEdit()
    {
        return cy.getById("edit-nti-uc-button");
    }

    private getClose()
    {
        return cy.getByTestId("close-nti-uc-button");
    }
}