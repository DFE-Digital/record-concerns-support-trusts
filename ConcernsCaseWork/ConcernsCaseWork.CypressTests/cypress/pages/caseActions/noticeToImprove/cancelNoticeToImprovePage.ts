import { Logger } from "../../../common/logger";

export class CancelNoticeToImprovePage {
    public withNotes(value: string): this {
        Logger.Log(`With notes ${value}`);

        cy.getById(`nti-notes`).clear().type(value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("have.value", value);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.Log(`With notes exceeding limit`);

        cy.getById('nti-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public cancel(): this
    {
        Logger.Log(`Confirming cancelling NTI`);

        cy.getById("cancel-nti-button").click();

        return this;
    }
}
