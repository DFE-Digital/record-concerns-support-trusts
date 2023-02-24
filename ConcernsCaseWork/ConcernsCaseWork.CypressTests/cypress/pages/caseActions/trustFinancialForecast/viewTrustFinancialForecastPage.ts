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

    public cannotEdit(): this {
        Logger.Log("Cannot edit");

        this.getEdit().should("not.exist");

        return this;
    }

    public cannotClose(): this {
        Logger.Log("Cannot close");

        this.getClose().should("not.exist");

        return this;
    }

    public edit(): this {
        Logger.Log("Editing trust financial forecast");

        this.getEdit().click();

        return this;
    }

    public close(): this {
        Logger.Log("Closing Trust financial forecast");

        this.getClose().click();

        return this;
    }

    private getEdit() {
        return cy.getById("trust-financial-forecast-edit-button");
    }

    private getClose() {
        return cy.getById("trust-financial-forecast-close-button");
    }
}