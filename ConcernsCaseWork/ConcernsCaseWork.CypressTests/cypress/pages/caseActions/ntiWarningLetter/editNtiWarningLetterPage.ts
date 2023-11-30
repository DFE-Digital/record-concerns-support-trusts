import { Logger } from "../../../common/logger";

export class EditNtiWarningLetterPage {
    public withStatus(value: string): this {
        Logger.log(`With status ${value}`);

        cy.getByTestId(`${value}`).check();

        return this;
    }

    public withDaySent(value: string): this {
        Logger.log(`With day sent ${value}`);

        cy.getById(`dtr-day-date-sent`).clear().type(value);

        return this;
    }

    public withMonthSent(value: string): this {
        Logger.log(`With month sent ${value}`);

        cy.getById(`dtr-month-date-sent`).clear().type(value);

        return this;
    }

    public withYearSent(value: string): this {
        Logger.log(`With year sent ${value}`);

        cy.getById(`dtr-year-date-sent`).clear().type(value);

        return this;
    }

    public withReason(value: string): this {
        Logger.log(`With reason ${value}`);

        cy.getByTestId(`reason-${value}`).check();

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

    public withCondition(value: string): this {
        Logger.log(`With condition ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public hasStatus(value: string): this {
        Logger.log(`Has status ${value}`);

        cy.getByTestId(`${value}`).should("be.checked");

        return this;
    }

    public hasDaySent(value: string): this {
        Logger.log(`Has day sent ${value}`);

        cy.getById(`dtr-day-date-sent`).should("contain.value", value);

        return this;
    }

    public hasMonthSent(value: string): this {
        Logger.log(`Has month sent ${value}`);

        cy.getById(`dtr-month-date-sent`).should("contain.value", value);

        return this;
    }

    public hasYearSent(value: string): this {
        Logger.log(`Has year sent ${value}`);

        cy.getById(`dtr-year-date-sent`).should("contain.value", value);

        return this;
    }

    public hasReason(value: string): this {
        Logger.log(`Has reason ${value}`);

        cy.getByTestId(`reason-${value}`).should("be.checked");

        return this;
    }

    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("contain.value", value);

        return this;
    }

    public hasCondition(value: string): this {
        Logger.log(`Has condition ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public clearReasons(): this
    {
        Logger.log("Clearing the reason fields");

        cy.get("[name='reason'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public clearConditions(): this
    {
        Logger.log("Clearing the conditions");

        cy.get("[name='condition'").each(element =>
            {
                cy.wrap(element).uncheck();
            });
    

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public editConditions(): this {
        Logger.log(`Editing conditions`);

        cy.getByTestId("edit-conditions-button").click();

        return this;
    }

    public save(): this {
        Logger.log("Saving the NTI warning letter");

        cy.getById("add-nti-wl-button").click();

        return this;
    }

    public saveConditions(): this {
        Logger.log("Saving the conditions");

        cy.getById("add-nti-conditions-wl-button").click();

        return this;
    }
}