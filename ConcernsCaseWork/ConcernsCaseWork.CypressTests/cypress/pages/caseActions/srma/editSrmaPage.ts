import { Logger } from "../../../common/logger";

export class EditSrmaPage {
    public withStatus(status: string) {
        Logger.Log(`With status {status}`);

        this.getStatus(status).click();

        return this;
    }

    public withDayTrustContacted(day: string): this {
        Logger.Log(`With day trust contacted ${day}`);

        cy.getById("dtr-day").clear().type(day);

        return this;
    }

    public withMonthTrustContacted(month: string): this {
        Logger.Log(`With month trust contacted ${month}`);

        cy.getById("dtr-month").clear().type(month);

        return this;
    }

    public withYearTrustContacted(year: string): this {
        Logger.Log(`With year trust contacted ${year}`);

        cy.getById("dtr-year").clear().type(year);

        return this;
    }

    public withNotes(notes: string) {
        Logger.Log(`With notes ${notes}`);

        cy.getById("srma-notes").clear().type(notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.Log(`With notes exceeding limit`);

        cy.getById('srma-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public withReason(reason: string): this {
        Logger.Log(`With SRMA reason`);

        this.getReason(reason).click();

        return this;
    }

    public withDayAccepted(value: string): this {
        Logger.Log(`With day accepted ${value}`);

        cy.getById(`dtr-day`).clear().type(value);

        return this;
    }

    public withMonthAccepted(value: string): this {
        Logger.Log(`With month accepted ${value}`);

        cy.getById(`dtr-month`).clear().type(value);

        return this;
    }

    public withYearAccepted(value: string): this {
        Logger.Log(`With year accepted ${value}`);

        cy.getById(`dtr-year`).clear().type(value);

        return this;
    }

    public withStartDayOfVisit(value: string): this {
        Logger.Log(`With start day of visit ${value}`);

        cy.getById(`start-dtr-day`).clear().type(value);

        return this;
    }

    public withStartMonthOfVisit(value: string): this {
        Logger.Log(`With start month of visit ${value}`);

        cy.getById(`start-dtr-month`).clear().type(value);

        return this;
    }

    public withStartYearOfVisit(value: string): this {
        Logger.Log(`With start year of visit ${value}`);

        cy.getById(`start-dtr-year`).clear().type(value);

        return this;
    }

    public withEndDayOfVisit(value: string): this {
        Logger.Log(`With end day of visit ${value}`);

        cy.getById(`end-dtr-day`).clear().type(value);

        return this;
    }

    public withEndMonthOfVisit(value: string): this {
        Logger.Log(`With end month of visit ${value}`);

        cy.getById(`end-dtr-month`).clear().type(value);

        return this;
    }

    public withEndYearOfVisit(value: string): this {
        Logger.Log(`With end year of visit ${value}`);

        cy.getById(`end-dtr-year`).clear().type(value);

        return this;
    }

    public withDayReportSentToTrust(value: string): this {
        Logger.Log(`With day report sent to trust ${value}`);

        cy.getById(`dtr-day`).clear().type(value);

        return this;
    }

    public withMonthReportSentToTrust(value: string): this {
        Logger.Log(`With month report sent to trust ${value}`);

        cy.getById(`dtr-month`).clear().type(value);

        return this;
    }

    public withYearReportSentToTrust(value: string): this {
        Logger.Log(`With year report sent to trust ${value}`);

        cy.getById(`dtr-year`).clear().type(value);

        return this;
    }

    public confirmComplete(): this
    {
        Logger.Log("Confirming the SRMA is complete");

        cy.getById("confirmChk").check();

        return this;
    }

    public confirmCancelled(): this
    {
        Logger.Log("Confirming the SRMA is cancelled");
        
        cy.getById("confirmChk").check();

        return this;
    }

    public confirmDeclined(): this
    {
        Logger.Log("Confirming the SRMA is declined");
        
        cy.getById("confirmChk").check();

        return this;
    }

    public hasStatus(value: string): this {
        Logger.Log(`With status ${value}`);

        this.getStatus(value).should("be.checked");

        return this;
    }

    public hasDayTrustContacted(value: string): this {
        Logger.Log(`Has day trust contacted ${value}`);

        cy.getById(`dtr-day`).should("contain.value", value);

        return this;
    }

    public hasMonthTrustContacted(value: string): this {
        Logger.Log(`Has month trust contacted ${value}`);

        cy.getById(`dtr-month`).should("contain.value", value);

        return this;
    }

    public hasYearTrustContacted(value: string): this {
        Logger.Log(`Has year trust contacted ${value}`);

        cy.getById(`dtr-year`).should("contain.value", value);

        return this;
    }

    public hasReason(value: string): this {
        Logger.Log(`Has reason ${value}`);

        this.getReason(value).should("be.checked", value);

        return this;
    }

    public hasDayAccepted(value: string): this {
        Logger.Log(`Has day accepted ${value}`);

        cy.getById(`dtr-day`).should("contain.value", value);

        return this;
    }

    public hasMonthAccepted(value: string): this {
        Logger.Log(`Has month trust contacted ${value}`);

        cy.getById(`dtr-month`).should("contain.value", value);

        return this;
    }

    public hasYearAccepted(value: string): this {
        Logger.Log(`Has year trust contacted ${value}`);

        cy.getById(`dtr-year`).should("contain.value", value);

        return this;
    }

    public hasStartDayOfVisit(value: string): this {
        Logger.Log(`Has start day of visit ${value}`);

        cy.getById(`start-dtr-day`).should("contain.value", value);

        return this;
    }

    public hasStartMonthOfVisit(value: string): this {
        Logger.Log(`Has start month of visit ${value}`);

        cy.getById(`start-dtr-month`).should("contain.value", value);

        return this;
    }

    public hasStartYearOfVisit(value: string): this {
        Logger.Log(`Has start year of visit ${value}`);

        cy.getById(`start-dtr-year`).should("contain.value", value);

        return this;
    }

    public hasEndDayOfVisit(value: string): this {
        Logger.Log(`Has end day of visit ${value}`);

        cy.getById(`end-dtr-day`).should("contain.value", value);

        return this;
    }

    public hasEndMonthOfVisit(value: string): this {
        Logger.Log(`Has end month of visit ${value}`);

        cy.getById(`end-dtr-month`).should("contain.value", value);

        return this;
    }

    public hasEndYearOfVisit(value: string): this {
        Logger.Log(`Has end year of visit ${value}`);

        cy.getById(`end-dtr-year`).should("contain.value", value);

        return this;
    }

    public hasDayReportSentToTrust(value: string): this {
        Logger.Log(`Has day report sent to trust ${value}`);

        cy.getById(`dtr-day`).should("contain.value", value);

        return this;
    }

    public hasMonthReportSentToTrust(value: string): this {
        Logger.Log(`Has month report sent to trust ${value}`);

        cy.getById(`dtr-month`).should("contain.value", value);

        return this;
    }

    public hasYearReportSentToTrust(value: string): this {
        Logger.Log(`Has year report sent to trust ${value}`);

        cy.getById(`dtr-year`).should("contain.value", value);

        return this;
    }
    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getById(`srma-notes`).should("contain.value", value);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public save(): this {
        Logger.Log("Saving SRMA");

        cy.getById("add-srma-button").click();

        return this;
    }

    private getStatus(status: string) {
        const id = status.split(" ").join("");

        return cy.getByTestId(id);
    }

    private getReason(reason: string) {
        const id = reason.split(" ").join("");

        return cy.getByTestId(id)
    }
}