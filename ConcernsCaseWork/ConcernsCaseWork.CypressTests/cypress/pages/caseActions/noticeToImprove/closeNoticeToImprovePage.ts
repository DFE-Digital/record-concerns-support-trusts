import { Logger } from "../../../common/logger";

export class CloseNoticeToImprovePage {
    public withDayClosed(value: string): this {
        Logger.Log(`With day closed ${value}`);

        cy.getById(`dtr-day`).clear().type(value);

        return this;
    }

    public withMonthClosed(value: string): this {
        Logger.Log(`With month closed ${value}`);

        cy.getById(`dtr-month`).clear().type(value);

        return this;
    }

    public withYearClosed(value: string): this {
        Logger.Log(`With year closed ${value}`);

        cy.getById(`dtr-year`).clear().type(value);

        return this;
    }

    public withNotes(value: string): this {
        Logger.Log(`With notes ${value}`);

        cy.getById('nti-notes').clear().type(value);

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

    public clearDateFields(): this {
        Logger.Log(`Clearing date fields`);

        cy.getById('dtr-day').clear();
        cy.getById('dtr-month').clear();
        cy.getById('dtr-year').clear();

        return this;
    }

    public close(): this {
        Logger.Log("Confirming Close of Notice To Improve");

        cy.getById("add-nti-wl-button").click();

        return this;
    }
}