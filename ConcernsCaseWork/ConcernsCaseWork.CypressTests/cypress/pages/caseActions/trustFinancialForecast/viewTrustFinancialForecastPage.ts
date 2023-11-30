import { Logger } from "../../../common/logger";


export class ViewTrustFinancialForecastPage
{
    public hasDateOpened(value: string) {
        Logger.log(`Has date opened ${value}`);

        cy.getByTestId("trust-financial-forecast-date-opened").should("contain.text", value);

        return this;
    }

    public hasForecastingTool(forecastingTool: string): this {
        Logger.log(`Has forecasting tool ${forecastingTool}`);

        cy.getByTestId(`trust-financial-forecast-when-run`).should("contain.text", forecastingTool);

        return this;
    }

    public hasInitialReviewDate(reviewDate: string): this {
        Logger.log(`Has initial review date ${reviewDate}`);

        cy.getByTestId(`trust-financial-forecast-when-reviewed`).should("contain.text", reviewDate);

        return this;
    }

    public hasTrustRespondedDate(respondedDate: string): this {
        Logger.log(`Has trust responded date ${respondedDate}`);

        cy.getByTestId(`trust-financial-forecast-when-responded`).should("contain.text", respondedDate);

        return this;
    }

    public hasTrustResponse(trustResponse: string): this {
        Logger.log(`Has trust response ${trustResponse}`);

        cy.getByTestId(`trust-financial-forecast-was-satisfactory`).should("contain.text", trustResponse);

        return this;
    }

    public hasSRMABeenOffered(response: string): this {
        Logger.log(`Has SRMA been offered ${response}`);

        cy.getByTestId(`trust-financial-forecast-srma-offered`).should("contain.text", response);

        return this;
    }

    public hasNotes(notes: string): this {
        Logger.log(`Has notes ${notes}`);

        cy.getByTestId(`trust-financial-forecast-notes`).should("contain.text", notes);

        return this;
    }

    public hasDateClosed(value: string) {
        Logger.log(`Has date closed ${value}`);

        cy.getByTestId("trust-financial-forecast-date-closed").should("contain.text", value);

        return this;
    }

    public canEdit(): this {
        Logger.log("Can edit");
        
        this.getEdit();

        return this;
    }

    public cannotEdit(): this {
        Logger.log("Cannot edit");

        this.getEdit().should("not.exist");

        return this;
    }

    public canClose(): this {
        Logger.log("Can close");

        this.getClose();

        return this;
    }

    public cannotClose(): this {
        Logger.log("Cannot close");

        this.getClose().should("not.exist");

        return this;
    }

    public edit(): this {
        Logger.log("Editing trust financial forecast");

        this.getEdit().click();

        return this;
    }

    public close(): this {
        Logger.log("Closing Trust financial forecast");

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