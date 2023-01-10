import { Logger } from "../../../common/logger";

export class EditNtiWarningLetterPage {
    public withStatus(value: string): this {
        Logger.Log(`With status ${value}`);

        cy.getByTestId(`status-${value}`).check();

        return this;
    }

    public withDaySent(value: string): this {
        Logger.Log(`With day sent ${value}`);

        cy.getById(`dtr-day`).clear().type(value);

        return this;
    }

    public withMonthSent(value: string): this {
        Logger.Log(`With month sent ${value}`);

        cy.getById(`dtr-month`).clear().type(value);

        return this;
    }

    public withYearSent(value: string): this {
        Logger.Log(`With year sent ${value}`);

        cy.getById(`dtr-year`).clear().type(value);

        return this;
    }

    public withReason(value: string): this {
        Logger.Log(`With reason ${value}`);

        cy.getByTestId(`reason-${value}`).check();

        return this;
    }

    public withNotes(value: string): this {
        Logger.Log(`With notes ${value}`);

        cy.getById(`nti-notes`).clear().type(value);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.Log(`With notes exceeding limit`);

        cy.getById('nti-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public withCondition(value: string): this {
        Logger.Log(`With condition ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`status-${value}`).should("be.checked");

        return this;
    }

    public hasDaySent(value: string): this {
        Logger.Log(`Has day sent ${value}`);

        cy.getById(`dtr-day`).should("contain.value", value);

        return this;
    }

    public hasMonthSent(value: string): this {
        Logger.Log(`Has month sent ${value}`);

        cy.getById(`dtr-month`).should("contain.value", value);

        return this;
    }

    public hasYearSent(value: string): this {
        Logger.Log(`Has year sent ${value}`);

        cy.getById(`dtr-year`).should("contain.value", value);

        return this;
    }

    public hasReason(value: string): this {
        Logger.Log(`Has reason ${value}`);

        cy.getByTestId(`reason-${value}`).should("be.checked");

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("contain.value", value);

        return this;
    }

    public hasCondition(value: string): this {
        Logger.Log(`Has condition ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public clearReasons(): this
    {
        Logger.Log("Clearing the reason fields");

        cy.get("[name='reason'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public clearConditions(): this
    {
        Logger.Log("Clearing the conditions");

        cy.get("[name='condition'").each(element =>
            {
                cy.wrap(element).uncheck();
            });
    

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public editConditions(): this {
        Logger.Log(`Editing conditions`);

        cy.getByTestId("edit-conditions-button").click();

        return this;
    }

    public save(): this {
        Logger.Log("Saving the NTI warning letter");

        cy.getById("add-nti-wl-button").click();

        return this;
    }

    public saveConditions(): this {
        Logger.Log("Saving the conditions");

        cy.getById("add-nti-conditions-wl-button").click();

        return this;
    }
}