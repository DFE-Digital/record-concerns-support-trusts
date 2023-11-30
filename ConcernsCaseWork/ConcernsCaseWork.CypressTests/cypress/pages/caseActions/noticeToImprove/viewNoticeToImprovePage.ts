import { Logger } from "../../../common/logger";

export class ViewNoticeToImprovePage {

    public hasDateOpened(value: string) {
        Logger.log(`Has date opened ${value}`);

        cy.getByTestId("date-opened-text").should("contain.text", value);

        return this;
    }

    public hasDateCompleted(value: string) {
        Logger.log(`Has date completed ${value}`);

        cy.getByTestId("date-completed-text").should("contain.text", value);

        return this;
    }

    public hasStatus(value: string): this {
        Logger.log(`Has status ${value}`);

        cy.getByTestId(`status-text`).should("contain.text", value);

        return this;
    }

    public hasDateIssued(value: string): this {
        Logger.log(`Has date issued ${value}`);

        cy.getByTestId(`date-issued-text`).should("contain.text", value);

        return this;
    }

    public hasReasonIssued(value: string): this {
        Logger.log(`Has reason issued ${value}`);

        cy.getByTestId(`reason-text`).should("contain.text", value);

        return this;
    }

    public hasConditions(value: string): this {
        Logger.log(`Has conditions ${value}`);

        cy.getByTestId(`condition-text`).should("contain.text", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getByTestId(`notes-text`).should("contains.text", value);

        return this;
    }

    public hasDateClosed(value: string): this
    {
        Logger.log(`Has date NTI closed ${value}`);

        cy.getByTestId("date-nti-closed").should("contains.text", value);

        return this;
    }

    public hasDateCancelled(value: string): this
    {
        Logger.log(`Has date NTI cancelled ${value}`);

        cy.getByTestId("date-nti-cancelled").should("contains.text", value);

        return this;
    }

    public hasDateLifted(value: string): this
    {
        Logger.log(`Has date NTI lifted ${value}`);

        cy.getByTestId("date-nti-lifted").should("contains.text", value);

        return this;
    }

    public hasSubmissionDecisionId(value: string): this
    {
        Logger.log(`Has submission decision ID ${value}`);

        cy.getByTestId("submission-decision-id").should("contains.text", value);

        return this;
    }

    public edit(): this
    {
        Logger.log("Editing Notice To Improve");

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

    public cancel(): this
    {
        Logger.log("Cancelling NTI");

        this.getCancel().click();

        return this;
    }

    public canCancel(): this
    {
        Logger.log("Can cancel");

        this.getCancel();

        return this;
    }

    public cannotCancel(): this
    {
        Logger.log("Cannot cancel");

        this.getCancel().should("not.exist");

        return this;
    }

    public close(): this {
        Logger.log("Closing Notice To Improve");

        this.getClose().click();

        return this;
    }

    public canClose(): this
    {
        Logger.log("Cannot close");

        this.getClose();

        return this;
    }

    public cannotClose(): this
    {
        Logger.log("Cannot close");

        this.getClose().should("not.exist");

        return this;
    }

    public lift(): this {
        Logger.log("Lifting Notice To Improve");

        this.getLift().click();

        return this;
    }

    public canLift(): this
    {
        Logger.log("Can lift");

        this.getLift();

        return this;
    }

    public cannotLift(): this
    {
        Logger.log("Cannot lift");

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