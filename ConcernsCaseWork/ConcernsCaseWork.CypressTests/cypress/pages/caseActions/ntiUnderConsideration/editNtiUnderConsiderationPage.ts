import { Logger } from "../../../common/logger";

export class EditNtiUnderConsiderationPage {
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

    public hasReason(value: string): this {
        Logger.log(`Has reason ${value}`);

        cy.getByTestId(`reason-${value}`).should("be.checked", value);

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

    public clearReasons(): this
    {
        Logger.log("Clearing the reasons");

        cy.get("[name='reason'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public save(): this {
        Logger.log("Saving NTI under consideration");

        cy.getById("add-nti-uc-button").click();

        return this;
    }
}