import { Logger } from "../../../common/logger";

export class CloseNoticeToImprovePage {
    public withDayClosed(value: string): this {
        Logger.log(`With day closed ${value}`);

        cy.getById(`dtr-day-date-nti-closed`).clear().type(value);

        return this;
    }

    public withMonthClosed(value: string): this {
        Logger.log(`With month closed ${value}`);

        cy.getById(`dtr-month-date-nti-closed`).clear().type(value);

        return this;
    }

    public withYearClosed(value: string): this {
        Logger.log(`With year closed ${value}`);

        cy.getById(`dtr-year-date-nti-closed`).clear().type(value);

        return this;
    }

    public withNotes(value: string): this {
        Logger.log(`With notes ${value}`);

        cy.getById('nti-notes').clear().type(value);

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

    public clearDateFields(): this {
        Logger.log(`Clearing date fields`);

        cy.getById('dtr-day-date-nti-closed').clear();
        cy.getById('dtr-month-date-nti-closed').clear();
        cy.getById('dtr-year-date-nti-closed').clear();

        return this;
    }

    public close(): this {
        Logger.log("Confirming Close of Notice To Improve");

        cy.getById("close-nti-button").click();

        return this;
    }
}