import { Logger } from "../../../common/logger";

export class EditNtiUnderConsiderationPage {
    public withReasons(value: string): this {
        Logger.Log(`With reasons ${value}`);

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

    public hasReasons(value: string): this {
        Logger.Log(`Has reasons ${value}`);

        cy.getByTestId(`reason-${value}`).should("be.checked", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("contain.value", value);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public clearReasons(): this
    {
        Logger.Log("Clearing the reasons");

        cy.get("[name='reason'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public save(): this {
        Logger.Log("Saving NTI under consideration");

        cy.getById("add-nti-uc-button").click();

        return this;
    }
}