import { Logger } from "../../../common/logger";


export class EditTrustFinancialForecastPage
{
    public withForecastingTool(forecastingTool: string): this {
        Logger.log(`With forecasting tool ${forecastingTool}`);

        cy.getByTestId(`${forecastingTool}`).check();

        return this;
    }

    public hasForecastingTool(forecastingTool: string): this {
        Logger.log(`Has forecasting tool ${forecastingTool}`);

        cy.getByTestId(`${forecastingTool}`).should("be.checked");

        return this;
    }

    public clearAllDates(): this
    {
        cy.getById(`dtr-day-sfso-initial-review-happened-at`).clear();
        cy.getById(`dtr-month-sfso-initial-review-happened-at`).clear();
        cy.getById(`dtr-year-sfso-initial-review-happened-at`).clear();
        cy.getById(`dtr-day-trust-responded-at`).clear();
        cy.getById(`dtr-month-trust-responded-at`).clear();
        cy.getById(`dtr-year-trust-responded-at`).clear();

        return this;
    }

    public withDayReviewHappened(value: string): this {
        Logger.log(`With day review happened ${value}`);

        cy.getById(`dtr-day-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public hasDayReviewHappened(value: string): this {
        Logger.log(`Has day review happened ${value}`);

        cy.getById(`dtr-day-sfso-initial-review-happened-at`).should("have.value", value);

        return this;
    }

    public withMonthReviewHappened(value: string): this {
        Logger.log(`With month review happen ${value}`);

        cy.getById(`dtr-month-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public hasMonthReviewHappened(value: string): this {
        Logger.log(`Has month review happen ${value}`);

        cy.getById(`dtr-month-sfso-initial-review-happened-at`).should("have.value", value);

        return this;
    }

    public withYearReviewHappened(value: string): this {
        Logger.log(`With year review happen ${value}`);

        cy.getById(`dtr-year-sfso-initial-review-happened-at`).clear().type(value);

        return this;
    }

    public hasYearReviewHappened(value: string): this {
        Logger.log(`With year review happen ${value}`);

        cy.getById(`dtr-year-sfso-initial-review-happened-at`).should("have.value", value);

        return this;
    }

    public withDayTrustResponded(value: string): this {
        Logger.log(`With day review happen ${value}`);

        cy.getById(`dtr-day-trust-responded-at`).clear().type(value);

        return this;
    }

    public hasDayTrustResponded(value: string): this {
        Logger.log(`Has day review happen ${value}`);

        cy.getById(`dtr-day-trust-responded-at`).should("have.value", value);

        return this;
    }

    public withMonthTrustResponded(value: string): this {
        Logger.log(`With month review happen ${value}`);

        cy.getById(`dtr-month-trust-responded-at`).clear().type(value);

        return this;
    }

    public hasMonthTrustResponded(value: string): this {
        Logger.log(`Has month review happen ${value}`);

        cy.getById(`dtr-month-trust-responded-at`).should("have.value", value);

        return this;
    }

    public withYearTrustResponded(value: string): this {
        Logger.log(`With year review happen ${value}`);

        cy.getById(`dtr-year-trust-responded-at`).clear().type(value);

        return this;
    }

    public hasYearTrustResponded(value: string): this {
        Logger.log(`With year review happen ${value}`);

        cy.getById(`dtr-year-trust-responded-at`).should("have.value", value);

        return this;
    }

    public withTrustResponseSatisfactory(trustResponseSatisfactory: string): this {
        Logger.log(`With trust response satisfactory ${trustResponseSatisfactory}`);

        cy.getByTestId(`${trustResponseSatisfactory}`).check();

        return this;
    }

    
    public hasTrustResponseSatisfactory(trustResponseSatisfactory: string): this {
        Logger.log(`Has trust response satisfactory ${trustResponseSatisfactory}`);

        cy.getByTestId(`${trustResponseSatisfactory}`).should("be.checked");

        return this;
    }

    public withSRMAOffered(srmaOffered: string): this {
        Logger.log(`With SRMA Offered ${srmaOffered}`);

        cy.getByTestId(`${srmaOffered}`).check();

        return this;
    }

    public hasSRMAOffered(srmaOffered: string): this {
        Logger.log(`Has SRMA Offered ${srmaOffered}`);

        cy.getByTestId(`${srmaOffered}`).should("be.checked");

        return this;
    }

    public withNotes(notes: string): this {
        Logger.log(`With Notes ${notes}`);

        cy.getByTestId(`notes`).clear().type(notes);

        return this;
    }

    public hasNotes(notes: string): this {
        Logger.log(`Has Notes ${notes}`);

        cy.getByTestId(`notes`).should("have.value", notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.log(`With notes exceeding limit`);

        cy.getByTestId('notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public save(): this {
        Logger.log("Saving the Trust financial forecast");

        cy.getByTestId("save-trust-financial-forecast-button").click();

        return this;
    }

    public close(): this {
        Logger.log("Close the Trust financial forecast");

        cy.getById("trust-financial-forecast-close-button").click();

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }
}