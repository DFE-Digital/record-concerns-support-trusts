import { Logger } from "../../../common/logger";


export class CloseTrustFinancialForecastPage
{
    public withNotes(notes: string): this {
        Logger.Log(`With Notes ${notes}`);

        cy.getByTestId(`notes`).clear().type(notes);

        return this;
    }

    public close(): this {
        Logger.Log("Saving the Trust financial forecast");

        cy.getById("close-trust-financial-forecast-button").click();

        return this;
    }

    public hasNotes(notes: string): this {
        Logger.Log(`Has Notes ${notes}`);

        cy.getByTestId(`notes`).should("contain.text", notes);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }
}