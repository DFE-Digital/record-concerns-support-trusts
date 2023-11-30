import { Logger } from "../../../common/logger";

export class CancelNoticeToImprovePage {
    public withNotes(value: string): this {
        Logger.log(`With notes ${value}`);

        cy.getById(`nti-notes`).clear().type(value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("have.value", value);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.log(`With notes exceeding limit`);

        cy.getById('nti-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public cancel(): this
    {
        Logger.log(`Confirming cancelling NTI`);

        cy.getById("cancel-nti-button").click();

        return this;
    }
}
