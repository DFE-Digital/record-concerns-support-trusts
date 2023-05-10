import { Logger } from "../../../common/logger";


export class EditTrustFinancialForecastPage
{
    public withForecastingTool(forecastingTool: string): this {
        Logger.Log(`With forecasting tool ${forecastingTool}`);

        cy.getByTestId(`${forecastingTool}`).check();

        return this;
    }

    public hasForecastingTool(forecastingTool: string): this {
        Logger.Log(`Has forecasting tool ${forecastingTool}`);

        cy.getByTestId(`${forecastingTool}`).should("be.checked");

        return this;
    }


    public withDayReviewHappened(value: string): this {
        Logger.Log(`With day review happened ${value}`);

        cy.getById(`dtr-day-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public hasDayReviewHappened(value: string): this {
        Logger.Log(`Has day review happened ${value}`);

        cy.getById(`dtr-day-sfso-initial-review-happened-at`).should("have.value", value);

        return this;
    }

    public withMonthReviewHappened(value: string): this {
        Logger.Log(`With month review happen ${value}`);

        cy.getById(`dtr-month-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public hasMonthReviewHappened(value: string): this {
        Logger.Log(`Has month review happen ${value}`);

        cy.getById(`dtr-month-sfso-initial-review-happened-at`).should("have.value", value);

        return this;
    }

    public withYearReviewHappened(value: string): this {
        Logger.Log(`With year review happen ${value}`);

        cy.getById(`dtr-year-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public hasYearReviewHappened(value: string): this {
        Logger.Log(`With year review happen ${value}`);

        cy.getById(`dtr-year-sfso-initial-review-happened-at`).should("have.value", value);

        return this;
    }

    public withDayTrustResponded(value: string): this {
        Logger.Log(`With day review happen ${value}`);

        cy.getById(`dtr-day-trust-responded-at`).clear().type(value);

        return this;
    }

    public hasDayTrustResponded(value: string): this {
        Logger.Log(`Has day review happen ${value}`);

        cy.getById(`dtr-day-trust-responded-at`).should("have.value", value);

        return this;
    }

    public withMonthTrustResponded(value: string): this {
        Logger.Log(`With month review happen ${value}`);

        cy.getById(`dtr-month-trust-responded-at`).clear().type(value);

        return this;
    }

    public hasMonthTrustResponded(value: string): this {
        Logger.Log(`Has month review happen ${value}`);

        cy.getById(`dtr-month-trust-responded-at`).should("have.value", value);

        return this;
    }

    public withYearTrustResponded(value: string): this {
        Logger.Log(`With year review happen ${value}`);

        cy.getById(`dtr-year-trust-responded-at`).clear().type(value);

        return this;
    }

    public hasYearTrustResponded(value: string): this {
        Logger.Log(`With year review happen ${value}`);

        cy.getById(`dtr-year-trust-responded-at`).should("have.value", value);

        return this;
    }

    public withTrustResponseSatisfactory(trustResponseSatisfactory: string): this {
        Logger.Log(`With trust response satisfactory ${trustResponseSatisfactory}`);

        cy.getByTestId(`${trustResponseSatisfactory}`).check();

        return this;
    }

    
    public hasTrustResponseSatisfactory(trustResponseSatisfactory: string): this {
        Logger.Log(`Has trust response satisfactory ${trustResponseSatisfactory}`);

        cy.getByTestId(`${trustResponseSatisfactory}`).should("be.checked");

        return this;
    }

    public withSRMAOffered(srmaOffered: string): this {
        Logger.Log(`With SRMA Offered ${srmaOffered}`);

        cy.getByTestId(`${srmaOffered}`).check();

        return this;
    }

    public hasSRMAOffered(srmaOffered: string): this {
        Logger.Log(`Has SRMA Offered ${srmaOffered}`);

        cy.getByTestId(`${srmaOffered}`).should("be.checked");

        return this;
    }

    public withNotes(notes: string): this {
        Logger.Log(`With Notes ${notes}`);

        cy.getByTestId(`notes`).clear().type(notes);

        return this;
    }

    public hasNotes(notes: string): this {
        Logger.Log(`Has Notes ${notes}`);

        cy.getByTestId(`notes`).should("have.value", notes);

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