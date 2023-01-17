import { Logger } from "../../../common/logger";

export class ViewSrmaPage {
    public addStatus(): this
    {
        Logger.Log("Adding status");

        this.getAddStatus().click();

        return this;
    }

    public canAddStatus()
    {
        Logger.Log("Can add status");

        this.getAddStatus();

        return this;
    }

    public cannotAddStatus()
    {
        Logger.Log("Cannot add status");

        this.getAddStatus().should("not.exist");

        return this;
    }

    public addDateTrustContacted(): this
    {
        Logger.Log("Adding date trust was contacted");

        this.getAddDateTrustContacted().click();

        return this;
    }

    public canAddDateTrustContacted(): this
    {
        Logger.Log("Can add the date trust was contacted");

        this.getAddDateTrustContacted();

        return this;
    }

    public cannotAddDateTrustContacted(): this
    {
        Logger.Log("Cannot add the date trust was contacted");

        this.getAddDateTrustContacted().click();

        return this;
    }

    public addReason(): this {
        Logger.Log("Adding reason");

        this.getAddReason().click();

        return this;
    }

    public canAddReason(): this {
        Logger.Log("Can add a reason");

        this.getAddReason();

        return this;
    }

    public cannotAddReason(): this {
        Logger.Log("Cannot add a reason");

        this.getAddReason().should("not.exist");

        return this;
    }

    public addDateAccepted(): this {
        Logger.Log("Adding date accepted");

        this.getAddDateAccepted().click();

        return this;
    }

    public canAddDateAccepted(): this {
        Logger.Log("Can add date accepted");

        this.getAddDateAccepted();

        return this;
    }

    public cannotAddDateAccepted(): this {
        Logger.Log("Cannot add date accepted");

        this.getAddDateAccepted().should("not.exist");

        return this;
    }

    public addDateOfVisit(): this {
        Logger.Log("Adding date accepted");

        this.getAddDateOfVisit().click();

        return this;
    }

    public canAddDateOfVisit(): this {
        Logger.Log("Can add date accepted");

        this.getAddDateOfVisit();

        return this;
    }

    public cannotAddDateOfVisit(): this {
        Logger.Log("Cannot add date accepted");

        this.getAddDateOfVisit().should("not.exist");

        return this;
    }

    public addDateReportSentToTrust(): this {
        Logger.Log("Adding date report sent to trust");

        this.getAddDateReportSendToTrust().click();

        return this;
    }

    public canAddDateReportSentToTrust(): this {
        Logger.Log("Can add date report sent to trust");

        this.getAddDateReportSendToTrust();

        return this;
    }

    public cannotAddDateReportSentToTrust(): this {
        Logger.Log("Cannot add date report sent to trust");

        this.getAddDateReportSendToTrust().should("not.exist");

        return this;
    }

    public addNotes(): this
    {
        Logger.Log("Adding notes");

        this.getAddNotes().click();

        return this;
    }

    public canAddNotes(): this
    {
        Logger.Log("Can add notes");

        this.getAddNotes();

        return this;
    }

    public cannotAddNotes(): this
    {
        Logger.Log("Cannot add notes");

        this.getAddNotes().should("not.exist");

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

        this.getCancel().click();

        return this;
    }

    public canCancel()
    {
        Logger.Log("Can cancel SRMA");

        this.getCancel();

        return this;
    }

    public cannotCancel()
    {
        Logger.Log("Cannot cancel SRMA");

        this.getCancel().should("not.exist");

        return this;
    }

    public decline()
    {
        Logger.Log("Declining SRMA");

        this.getDecline().click();

        return this;
    }

    public canDecline()
    {
        Logger.Log("Can decline SRMA");

        this.getDecline();

        return this;
    }

    public cannotDecline()
    {
        Logger.Log("Cannot decline SRMA");

        this.getDecline().should("not.exist");

        return this;
    }

    public save(): this {
        Logger.Log("Saving SRMA");

        cy.getById("add-srma-button").click();

        return this;
    }

    private getAddStatus()
    {
        return cy.getByTestId("SRMA status");
    }

    private getAddDateTrustContacted()
    {
        return cy.getByTestId("date trust was contacted about SRMA");
    }

    private getAddReason() {
        return cy.getByTestId("SRMA reason")
    }

    private getAddDateAccepted()
    {
        return cy.getByTestId("SRMA date accepted");
    }

    private getAddDateOfVisit()
    {
        return cy.getByTestId("SRMA dates of visit");
    }

    private getAddDateReportSendToTrust() {
        return cy.getByTestId("date SRMA report sent to trust");
    }

    private getAddNotes() {
        return cy.getByTestId("SRMA notes");
    }

    private getCancel() {
       return cy.getByTestId("cancel-srma-button"); 
    }

    private getDecline() {
        return cy.getById("complete-decline-srma-button");
    }
}