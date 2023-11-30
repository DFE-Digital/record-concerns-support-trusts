import { Logger } from "../../../common/logger";

export class ViewNtiUnderConsiderationPage
{
    public hasDateOpened(value: string): this {

        Logger.log(`Has date opened ${value}`);

        cy.getByTestId("date-opened-text").should("contain.text", value); 

        return this;
    }

    public hasDateClosed(value: string): this {

        Logger.log(`Has date closed ${value}`);

        cy.getByTestId("date-closed-text").should("contain.text", value); 

        return this;
    }

    public hasReason(value: string): this {
        Logger.log(`Has reason ${value}`);

        cy.getByTestId(`nti-reasons`).should("contain.text", value);

        return this;
    }

    public hasReasonCount(value: number): this {
        Logger.log(`Has reason count ${value}`);

        cy.getByTestId(`nti-reasons`).children().should("have.length", value);

        return this;
    }

    public hasStatus(value: string): this {
        Logger.log(`Has status ${value}`);

        cy.getByTestId(`nti-status`).should("contain.text", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getByTestId(`nti-notes`).should("contain.text", value);

        return this;
    }

    public edit(): this
    {
        Logger.log("Editing NTI under consideration");

        this.getEdit().click();

        return this;
    }

    public canEdit(): this
    {
        Logger.log("Can edit");

        this.getEdit();

        return this;
    }

    public cannotEdit(): this
    {
        Logger.log("Cannot edit");

        this.getEdit().should("not.exist");

        return this;
    }

    public close(): this
    {
        Logger.log("Closing NTI under consideration");

        this.getClose().click();

        return this;
    }

    public canClose(): this
    {
        Logger.log("Can close");

        this.getClose();

        return this;
    }

    public cannotClose(): this
    {
        Logger.log("Cannot close");

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