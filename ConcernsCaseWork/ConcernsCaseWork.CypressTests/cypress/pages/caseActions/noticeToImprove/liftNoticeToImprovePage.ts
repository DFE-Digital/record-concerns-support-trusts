import { Logger } from "../../../common/logger";

export class LiftNoticeToImprovePage {
    public withSubmissionDecisionId(value: string): this {
        Logger.Log(`With submission decision id ${value}`);

        cy.getById(`submission-decision-id`).clear().type(value);

        return this;
    }

    public withDayLifted(value: string): this {
        Logger.Log(`With day lifted ${value}`);

        cy.getById(`dtr-day`).clear().type(value);

        return this;
    }

    public withMonthLifted(value: string): this {
        Logger.Log(`With month lifted ${value}`);

        cy.getById(`dtr-month`).clear().type(value);

        return this;
    }

    public withYearLifted(value: string): this {
        Logger.Log(`With year lifted ${value}`);

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

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("have.value", value);

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

    public lift(): this {
        Logger.Log("Lifting Notice To Improve");

        cy.getById("lift-nti-button").click();

        return this;
    }
}
