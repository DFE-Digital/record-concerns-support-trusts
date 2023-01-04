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

        cy.getByTestId("edit-nti-button").click();

        return this;
    }

    public cancel(): this
    {
        Logger.Log("Cancelling NTI");

        cy.getByTestId("cancel-nti-button").click();

        return this;
    }

    public close(): this {
        Logger.Log("Closing Notice To Improve");

        cy.getByTestId("close-nti-button").click();

        return this;
    }

    public lift(): this {
        Logger.Log("Lifting Notice To Improve");

        cy.getByTestId("lift-nti-button").click();

        return this;
    }

    public hasNoEditButton(): this
    {
        Logger.Log("Has no edit button");

        cy.getByTestId("edit-nti-button").should("not.exist");

        return this;
    }

    public hasNoLiftButton(): this
    {
        Logger.Log("Has no lift button");

        cy.getByTestId("lift-nti-button").should("not.exist");

        return this;
    }

    public hasNoCancelButton(): this
    {
        Logger.Log("Has no cancel button");

        cy.getByTestId("cancel-nti-button").should("not.exist");

        return this;
    }

    public hasNoCloseButton(): this
    {
        Logger.Log("Has no close button");

        cy.getByTestId("close-nti-button").should("not.exist");

        return this;
    }
}