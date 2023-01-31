import { Logger } from "../../../common/logger";


export class EditTrustFinancialForecastPage
{
    public withForecastingTool(forecastingTool: string): this {
        Logger.Log(`With forecasting tool ${forecastingTool}`);

        cy.getByTestId(`${forecastingTool}`).check();

        return this;
    }

    public withDayReviewHappened(value: string): this {
        Logger.Log(`With day reveiw happen ${value}`);

        cy.getById(`dtr-day-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public withMonthReviewHappened(value: string): this {
        Logger.Log(`With month reveiw happen ${value}`);

        cy.getById(`dtr-month-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public withYearReviewHappened(value: string): this {
        Logger.Log(`With year reveiw happen ${value}`);

        cy.getById(`dtr-year-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public withDayTrustResponded(value: string): this {
        Logger.Log(`With day reveiw happen ${value}`);

        cy.getById(`dtr-day-trust-responded-at`).clear().type(value);

        return this;
    }

    public withMonthTrustResponded(value: string): this {
        Logger.Log(`With month reveiw happen ${value}`);

        cy.getById(`dtr-month-trust-responded-at`).clear().type(value);

        return this;
    }

    public withYearTrustResponded(value: string): this {
        Logger.Log(`With year reveiw happen ${value}`);

        cy.getById(`dtr-year-trust-responded-at`).clear().type(value);

        return this;
    }

    public withTrustResponseSatisfactory(trustResponseSatisfactory: string): this {
        Logger.Log(`With trust response satisfactory ${trustResponseSatisfactory}`);

        cy.getByTestId(`${trustResponseSatisfactory}`).check();

        return this;
    }

    public withSRMAOffered(srmaOffered: string): this {
        Logger.Log(`With SRMA Offered ${srmaOffered}`);

        cy.getByTestId(`${srmaOffered}`).check();

        return this;
    }

    public withNotes(notes: string): this {
        Logger.Log(`With Notes ${notes}`);

        cy.getByTestId(`notes`).clear().type(notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.Log(`With notes exceeding limit`);

        cy.getByTestId('notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public save(): this {
        Logger.Log("Saving the Trust financial forecast");

        cy.getByTestId("save-trust-financial-forecast-button").click();

        return this;
    }

    public close(): this {
        Logger.Log("Close the Trust financial forecast");

        cy.getById("trust-financial-forecast-close-button").click();

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }
}