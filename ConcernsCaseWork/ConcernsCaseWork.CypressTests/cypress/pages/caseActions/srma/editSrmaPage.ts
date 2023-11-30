import { Logger } from "../../../common/logger";

export class EditSrmaPage {
    public withStatus(status: string) {
        Logger.log(`With status {status}`);

        this.getStatus(status).click();

        return this;
    }

    public withDayTrustContacted(day: string): this {
        Logger.log(`With day trust contacted ${day}`);

        cy.getByTestId("dtr-day-date-offered").clear().type(day);

        return this;
    }

    public withMonthTrustContacted(month: string): this {
        Logger.log(`With month trust contacted ${month}`);

        cy.getByTestId("dtr-month-date-offered").clear().type(month);

        return this;
    }

    public withYearTrustContacted(year: string): this {
        Logger.log(`With year trust contacted ${year}`);

        cy.getByTestId("dtr-year-date-offered").clear().type(year);

        return this;
    }

    public clearDateTrustContacted(): this
    {
        cy.getByTestId("dtr-day-date-offered").clear();
        cy.getByTestId("dtr-month-date-offered").clear();
        cy.getByTestId("dtr-year-date-offered").clear();

        return this;
    }

    public clearDateOfVisit(): this
    {
        cy.getByTestId("dtr-day-start").clear();
        cy.getByTestId("dtr-month-start").clear();
        cy.getByTestId("dtr-year-start").clear();

        cy.getByTestId("dtr-day-end").clear();
        cy.getByTestId("dtr-month-end").clear();
        cy.getByTestId("dtr-year-end").clear();

        return this;
    }

    public withNotes(notes: string) {
        Logger.log(`With notes ${notes}`);

        cy.getById("srma-notes").clear({ force: true }).type(notes);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.log(`With notes exceeding limit`);

        cy.getById('srma-notes').clear().invoke("val", "x 1 D".repeat(1001));

        return this;
    }

    public withReason(reason: string): this {
        Logger.log(`With SRMA reason`);

        this.getReason(reason).click();

        return this;
    }

    public withDayAccepted(value: string): this {
        Logger.log(`With day accepted ${value}`);

        cy.getById(`dtr-day-date-accepted`).clear().type(value);

        return this;
    }

    public withMonthAccepted(value: string): this {
        Logger.log(`With month accepted ${value}`);

        cy.getById(`dtr-month-date-accepted`).clear().type(value);

        return this;
    }

    public withYearAccepted(value: string): this {
        Logger.log(`With year accepted ${value}`);

        cy.getById(`dtr-year-date-accepted`).clear().type(value);

        return this;
    }

    public withStartDayOfVisit(value: string): this {
        Logger.log(`With start day of visit ${value}`);

        cy.getById(`dtr-day-start`).clear().type(value);

        return this;
    }

    public withStartMonthOfVisit(value: string): this {
        Logger.log(`With start month of visit ${value}`);

        cy.getById(`dtr-month-start`).clear().type(value);

        return this;
    }

    public withStartYearOfVisit(value: string): this {
        Logger.log(`With start year of visit ${value}`);

        cy.getById(`dtr-year-start`).clear().type(value);

        return this;
    }

    public withEndDayOfVisit(value: string): this {
        Logger.log(`With end day of visit ${value}`);

        cy.getById(`dtr-day-end`).clear().type(value);

        return this;
    }

    public withEndMonthOfVisit(value: string): this {
        Logger.log(`With end month of visit ${value}`);

        cy.getById(`dtr-month-end`).clear().type(value);

        return this;
    }

    public withEndYearOfVisit(value: string): this {
        Logger.log(`With end year of visit ${value}`);

        cy.getById(`dtr-year-end`).clear().type(value);

        return this;
    }

    public withDayReportSentToTrust(value: string): this {
        Logger.log(`With day report sent to trust ${value}`);

        cy.getById(`dtr-day-date-report-sent`).clear().type(value);

        return this;
    }

    public withMonthReportSentToTrust(value: string): this {
        Logger.log(`With month report sent to trust ${value}`);

        cy.getById(`dtr-month-date-report-sent`).clear().type(value);

        return this;
    }

    public withYearReportSentToTrust(value: string): this {
        Logger.log(`With year report sent to trust ${value}`);

        cy.getById(`dtr-year-date-report-sent`).clear().type(value);

        return this;
    }

    public confirmComplete(): this
    {
        Logger.log("Confirming the SRMA is complete");

        cy.getById("srma-confirm-check").check();

        return this;
    }

    public confirmCancelled(): this
    {
        Logger.log("Confirming the SRMA is cancelled");
        
        cy.getById("srma-confirm-check").check();

        return this;
    }

    public confirmDeclined(): this
    {
        Logger.log("Confirming the SRMA is declined");
        
        cy.getById("srma-confirm-check").check();

        return this;
    }

    public hasStatus(value: string): this {
        Logger.log(`With status ${value}`);

        this.getStatus(value).should("be.checked");

        return this;
    }

    public hasDayTrustContacted(value: string): this {
        Logger.log(`Has day trust contacted ${value}`);

        cy.getById(`dtr-day-date-offered`).should("contain.value", value);

        return this;
    }

    public hasMonthTrustContacted(value: string): this {
        Logger.log(`Has month trust contacted ${value}`);

        cy.getById(`dtr-month-date-offered`).should("contain.value", value);

        return this;
    }

    public hasYearTrustContacted(value: string): this {
        Logger.log(`Has year trust contacted ${value}`);

        cy.getById(`dtr-year-date-offered`).should("contain.value", value);

        return this;
    }

    public hasReason(value: string): this {
        Logger.log(`Has reason ${value}`);

        this.getReason(value).should("be.checked", value);

        return this;
    }

    public hasDayAccepted(value: string): this {
        Logger.log(`Has day accepted ${value}`);

        cy.getById(`dtr-day-date-accepted`).should("contain.value", value);

        return this;
    }

    public hasMonthAccepted(value: string): this {
        Logger.log(`Has month trust contacted ${value}`);

        cy.getById(`dtr-month-date-accepted`).should("contain.value", value);

        return this;
    }

    public hasYearAccepted(value: string): this {
        Logger.log(`Has year trust contacted ${value}`);

        cy.getById(`dtr-year-date-accepted`).should("contain.value", value);

        return this;
    }

    public hasStartDayOfVisit(value: string): this {
        Logger.log(`Has start day of visit ${value}`);

        cy.getById(`dtr-day-start`).should("contain.value", value);

        return this;
    }

    public hasStartMonthOfVisit(value: string): this {
        Logger.log(`Has start month of visit ${value}`);

        cy.getById(`dtr-month-start`).should("contain.value", value);

        return this;
    }

    public hasStartYearOfVisit(value: string): this {
        Logger.log(`Has start year of visit ${value}`);

        cy.getById(`dtr-year-start`).should("contain.value", value);

        return this;
    }

    public hasEndDayOfVisit(value: string): this {
        Logger.log(`Has end day of visit ${value}`);

        cy.getById(`dtr-day-end`).should("contain.value", value);

        return this;
    }

    public hasEndMonthOfVisit(value: string): this {
        Logger.log(`Has end month of visit ${value}`);

        cy.getById(`dtr-month-end`).should("contain.value", value);

        return this;
    }

    public hasEndYearOfVisit(value: string): this {
        Logger.log(`Has end year of visit ${value}`);

        cy.getById(`dtr-year-end`).should("contain.value", value);

        return this;
    }

    public hasDayReportSentToTrust(value: string): this {
        Logger.log(`Has day report sent to trust ${value}`);

        cy.getById(`dtr-day-date-report-sent`).should("contain.value", value);

        return this;
    }

    public hasMonthReportSentToTrust(value: string): this {
        Logger.log(`Has month report sent to trust ${value}`);

        cy.getById(`dtr-month-date-report-sent`).should("contain.value", value);

        return this;
    }

    public hasYearReportSentToTrust(value: string): this {
        Logger.log(`Has year report sent to trust ${value}`);

        cy.getById(`dtr-year-date-report-sent`).should("contain.value", value);

        return this;
    }
    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getById(`srma-notes`).should("contain.value", value);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public save(): this {
        Logger.log("Saving SRMA");

        cy.getById("add-srma-button").click();

        return this;
    }

    public hasReasonHintText(): this
    {
        Logger.log("Verify Text Hint ");
        
        this.getReasonHintText().should("exist");

        return this;
    }

    public hasNoReasonHintText(): this
    {
        Logger.log("Verify Text Hint is not displayed");
        
        this.getReasonHintText().should("not.exist");

        return this;
    }

    private getStatus(status: string) {
        return cy.getByTestId(status);
    }

    private getReason(reason: string) {
        const id = reason.split(" ").join("");

        return cy.getByTestId(id)
    }

    private getReasonHintText(): Cypress.Chainable<Element>
    {
        return cy.getByTestId('reason-hint-text');
    }
}