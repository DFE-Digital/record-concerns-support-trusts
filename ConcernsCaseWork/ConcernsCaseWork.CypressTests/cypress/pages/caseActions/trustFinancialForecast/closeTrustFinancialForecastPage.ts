import { Logger } from "../../../common/logger";


export class CloseTrustFinancialForecastPage
{
    public withNotes(notes: string): this {
        Logger.log(`With Notes ${notes}`);

        cy.getByTestId(`notes`).clear().type(notes);

        return this;
    }

    public close(): this {
        Logger.log("Saving the Trust financial forecast");

        cy.getById("close-trust-financial-forecast-button").click();

        return this;
    }

    public hasNotes(notes: string): this {
        Logger.log(`Has Notes ${notes}`);

        cy.getByTestId(`notes`).should("contain.text", notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.log(`With notes exceeding limit`);

        cy.getByTestId('notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }
}