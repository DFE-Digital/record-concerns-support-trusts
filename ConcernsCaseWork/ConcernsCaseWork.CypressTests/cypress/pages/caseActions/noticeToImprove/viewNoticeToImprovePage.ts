import { Logger } from "../../../common/logger";

export class ViewNoticeToImprovePage {
    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`status-text`).should("contain.text", value);

        return this;
    }

    public hasDateIssued(value: string): this {
        Logger.Log(`Has date issued ${value}`);

        cy.getByTestId(`date-issued-text`).should("contain.text", value);

        return this;
    }

    public hasReasonIssued(value: string): this {
        Logger.Log(`Has reason issued ${value}`);

        cy.getByTestId(`reason-text`).should("contain.text", value);

        return this;
    }

    public hasConditions(value: string): this {
        Logger.Log(`Has conditions ${value}`);

        cy.getByTestId(`condition-text`).should("contain.text", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getByTestId(`notes-text`).should("contains.text", value);

        return this;
    }

    public hasDateClosed(value: string): this
    {
        Logger.Log(`Has date NTI closed ${value}`);

        cy.getByTestId("date-nti-closed").should("contains.text", value);

        return this;
    }

    public edit(): this
    {
        Logger.Log("Editing Notice To Improve");

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

    public cancel(): this
    {
        Logger.Log("Cancelling NTI");

        this.getCancel().click();

        return this;
    }

    public canCancel(): this
    {
        Logger.Log("Can cancel");

        this.getCancel();

        return this;
    }

    public cannotCancel(): this
    {
        Logger.Log("Cannot cancel");

        this.getCancel().should("not.exist");

        return this;
    }

    public close(): this {
        Logger.Log("Closing Notice To Improve");

        this.getClose().click();

        return this;
    }

    public canClose(): this
    {
        Logger.Log("Cannot close");

        this.getClose();

        return this;
    }

    public cannotClose(): this
    {
        Logger.Log("Cannot close");

        this.getClose().should("not.exist");

        return this;
    }

    public lift(): this {
        Logger.Log("Lifting Notice To Improve");

        this.getLift().click();

        return this;
    }

    public canLift(): this
    {
        Logger.Log("Can lift");

        this.getLift();

        return this;
    }

    public cannotLift(): this
    {
        Logger.Log("Cannot lift");

        this.getLift().should("not.exist");

        return this;
    }

    private getEdit() {
        return cy.getByTestId("edit-nti-button");
    }

    private getCancel() {
        return cy.getByTestId("cancel-nti-button");
    }

    private getClose() {
        return cy.getByTestId("close-nti-button");
    }

    private getLift() {
        return cy.getByTestId("lift-nti-button");
    }
}