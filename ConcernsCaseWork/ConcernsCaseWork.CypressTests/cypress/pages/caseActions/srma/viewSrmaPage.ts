import { Logger } from "../../../common/logger";

export class ViewSrmaPage {
    public addStatus(): this
    {
        Logger.Log("Adding status");

        cy.getByTestId("SRMA status").click();

        return this;
    }

    public addDateTrustContacted(): this
    {
        Logger.Log("Adding date trust was contacted");

        cy.getByTestId("date trust was contacted about SRMA").click();

        return this;
    }

    public addReason(): this {
        Logger.Log("Adding reason");

        cy.getByTestId("SRMA reason").click();

        return this;
    }

    public addDateAccepted(): this {
        Logger.Log("Adding date accepted");

        cy.getByTestId("SRMA date accepted").click();

        return this;
    }

    public addDateOfVisit(): this {
        Logger.Log("Adding date accepted");

        cy.getByTestId("SRMA dates of visit").click();

        return this;
    }

    public addDateReportSentToTrust(): this {
        Logger.Log("Adding date report sent to trust");

        cy.getByTestId("date SRMA report sent to trust").click();

        return this;
    }

    public addNotes(): this
    {
        Logger.Log("Adding notes");

        cy.getByTestId("SRMA notes").click();

        return this;
    }

    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`status`).should("contain.text", value);

        return this;
    }

    public hasDateTrustContacted(value: string): this {
        Logger.Log(`Has date trust contacted ${value}`);

        cy.getByTestId(`date-trust-contacted`).should("contain.text", value);

        return this;
    }

    public hasReason(value: string): this {
        Logger.Log(`Has reason ${value}`);

        cy.getByTestId(`reason`).should("contain.text", value);

        return this;
    }

    public hasDateAccepted(value: string): this {
        Logger.Log(`Has date accepted ${value}`);

        cy.getByTestId(`date-accepted`).should("contain.text", value);

        return this;
    }

    public hasDateOfVisit(value: string): this {
        Logger.Log(`Has date of visit ${value}`);

        cy.getByTestId(`date-of-visit`).should("contain.text", value);

        return this;
    }

    public hasDateReportSentToTrust(value: string): this {
        Logger.Log(`Has date report sent to trust ${value}`);

        cy.getByTestId(`date-report-sent-to-trust`).should("contain.text", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getByTestId(`notes`).should("contain.text", value);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public resolve(): this
    {
        Logger.Log("Resolving the SRMA");

        cy.getById("complete-decline-srma-button").click();

        return this;
    }

    public cancel(): this {
        Logger.Log("Cancelling SRMA");

        cy.getByTestId("cancel-srma-button").click();

        return this;
    }

    public decline()
    {
        Logger.Log("Declining SRMA");

        cy.getById("complete-decline-srma-button").click();

        return this;
    }

    public save(): this {
        Logger.Log("Saving SRMA");

        cy.getById("add-srma-button").click();

        return this;
    }
}