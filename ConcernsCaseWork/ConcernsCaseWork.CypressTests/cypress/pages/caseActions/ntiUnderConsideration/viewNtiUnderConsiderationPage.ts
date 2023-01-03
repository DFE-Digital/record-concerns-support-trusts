import { Logger } from "../../../common/logger";

export class ViewNtiUnderConsiderationPage
{
    public hasReasons(value: string): this {
        Logger.Log(`Has reasons ${value}`);

        cy.getByTestId(`nti-reasons`).should("contain.text", value);

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

    public close(): this
    {
        Logger.Log("Closing NTI under consideration");

        this.getClose().click();

        return this;
    }

    public cannotEdit(): this
    {
        Logger.Log("Ensure that we cannot edit");

        this.getEdit().should("not.exist");

        return this;
    }

    public cannotClose(): this
    {
        Logger.Log("Ensure that we cannot close");

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