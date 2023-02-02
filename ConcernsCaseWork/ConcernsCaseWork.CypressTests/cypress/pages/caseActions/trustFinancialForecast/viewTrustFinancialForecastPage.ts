import { Logger } from "../../../common/logger";


export class ViewTrustFinancialForecastPage
{
    public hasForecastingTool(forecastingTool: string): this {
        Logger.Log(`Has forecasting tool ${forecastingTool}`);

        cy.getByTestId(`trust-financial-forecast-when-run`).should("contain.text", forecastingTool);

        return this;
    }

    public hasInitialReviewDate(reviewDate: string): this {
        Logger.Log(`Has initial review date ${reviewDate}`);

        cy.getByTestId(`trust-financial-forecast-when-reviewed`).should("contain.text", reviewDate);

        return this;
    }

    public hasTrustRespondedDate(respondedDate: string): this {
        Logger.Log(`Has trust responded date ${respondedDate}`);

        cy.getByTestId(`trust-financial-forecast-when-responded`).should("contain.text", respondedDate);

        return this;
    }

    public hasTrustResponse(trustResponse: string): this {
        Logger.Log(`Has trust response ${trustResponse}`);

        cy.getByTestId(`trust-financial-forecast-was-satisfactory`).should("contain.text", trustResponse);

        return this;
    }

    public hasSRMABeenOffered(response: string): this {
        Logger.Log(`Has SRMA been offered ${response}`);

        cy.getByTestId(`trust-financial-forecast-srma-offered`).should("contain.text", response);

        return this;
    }

    public hasNotes(notes: string): this {
        Logger.Log(`Has notes ${notes}`);

        cy.getByTestId(`trust-financial-forecast-notes`).should("contain.text", notes);

        return this;
    }

    public edit(): this {
        Logger.Log("Editing trust financial forecast");

        cy.getById("trust-financial-forecast-edit-button").click();

        return this;
    }


    
    /*

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

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }


    */
}