import { Logger } from "../../../common/logger";

export class CloseNtiUnderConsiderationPage
{
    public withStatus(status: string): this
    {
        Logger.log(`With closure status ${status}`);

        cy.getByTestId(status).click();

        return this;
    }

    public withNotes(value: string): this {
        Logger.log(`With notes ${value}`);

        cy.getById(`nti-notes`).clear().type(value);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.log(`With notes exceeding limit`);

        cy.getById('nti-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("contain.value", value);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public close(): this
    {
        Logger.log("Closing the NTI under consideration");

        cy.getById("add-nti-uc-button").click();

        return this;
    }
}