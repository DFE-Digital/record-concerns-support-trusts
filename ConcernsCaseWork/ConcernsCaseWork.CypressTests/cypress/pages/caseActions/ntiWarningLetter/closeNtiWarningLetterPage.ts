import { Logger } from "../../../common/logger";

export class CloseNtiWarningLetterPage
{
    public withReason(reason: string): this
    {
        Logger.Log(`With reason ${reason}`);

        cy.getByTestId(reason).click();

        return this;
    }

    public withNotes(notes: string)
    {
        Logger.Log(`With notes ${notes}`);

        cy.getById("nti-notes").clear().type(notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.Log(`With notes exceeding limit`);

        cy.getById('nti-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public hasNotes(notes: string)
    {
        Logger.Log(`Has notes ${notes}`);

        cy.getById("nti-notes").should("contain.value", notes);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public close(): this
    {
        Logger.Log("Confirmingh close of NTI warning letter");

        cy.getById("add-nti-wl-button").click();

        return this;
    }
}