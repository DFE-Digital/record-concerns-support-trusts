import { Logger } from "../../../common/logger";

export class ViewSrmaPage {
    public addStatus(): this
    {
        Logger.log("Adding status");

        this.getAddStatus().click();

        return this;
    }

    public canAddStatus()
    {
        Logger.log("Can add status");

        this.getAddStatus();

        return this;
    }

    public cannotAddStatus()
    {
        Logger.log("Cannot add status");

        this.getAddStatus().should("not.exist");

        return this;
    }

    public addDateTrustContacted(): this
    {
        Logger.log("Adding date trust was contacted");

        this.getAddDateTrustContacted().click();

        return this;
    }

    public canAddDateTrustContacted(): this
    {
        Logger.log("Can add the date trust was contacted");

        this.getAddDateTrustContacted();

        return this;
    }

    public cannotAddDateTrustContacted(): this
    {
        Logger.log("Cannot add the date trust was contacted");

        this.getAddDateTrustContacted().should("not.exist");

        return this;
    }

    public addReason(): this {
        Logger.log("Adding reason");

        this.getAddReason().click();

        return this;
    }

    public canAddReason(): this {
        Logger.log("Can add a reason");

        this.getAddReason();

        return this;
    }

    public cannotAddReason(): this {
        Logger.log("Cannot add a reason");

        this.getAddReason().should("not.exist");

        return this;
    }

    public addDateAccepted(): this {
        Logger.log("Adding date accepted");

        this.getAddDateAccepted().click();

        return this;
    }

    public canAddDateAccepted(): this {
        Logger.log("Can add date accepted");

        this.getAddDateAccepted();

        return this;
    }

    public cannotAddDateAccepted(): this {
        Logger.log("Cannot add date accepted");

        this.getAddDateAccepted().should("not.exist");

        return this;
    }

    public addDateOfVisit(): this {
        Logger.log("Adding date accepted");

        this.getAddDateOfVisit().click();

        return this;
    }

    public canAddDateOfVisit(): this {
        Logger.log("Can add date accepted");

        this.getAddDateOfVisit();

        return this;
    }

    public cannotAddDateOfVisit(): this {
        Logger.log("Cannot add date accepted");

        this.getAddDateOfVisit().should("not.exist");

        return this;
    }

    public addDateReportSentToTrust(): this {
        Logger.log("Adding date report sent to trust");

        this.getAddDateReportSendToTrust().click();

        return this;
    }

    public canAddDateReportSentToTrust(): this {
        Logger.log("Can add date report sent to trust");

        this.getAddDateReportSendToTrust();

        return this;
    }

    public cannotAddDateReportSentToTrust(): this {
        Logger.log("Cannot add date report sent to trust");

        this.getAddDateReportSendToTrust().should("not.exist");

        return this;
    }

    public addNotes(): this
    {
        Logger.log("Adding notes");

        this.getAddNotes().click();

        return this;
    }

    public canAddNotes(): this
    {
        Logger.log("Can add notes");

        this.getAddNotes();

        return this;
    }

    public cannotAddNotes(): this
    {
        Logger.log("Cannot add notes");

        this.getAddNotes().should("not.exist");

        return this;
    }

    public hasDateOpened(value: string) : this {
        Logger.log(`Has date opened ${value}`);

        cy.getByTestId("date-opened-text").should("contain.text", value);

        return this;
    }

    public hasDateClosed(value: string) : this {
        Logger.log(`Has date closed ${value}`);

        cy.getByTestId("date-closed-text").should("contain.text", value);

        return this;
    }

    public hasStatus(value: string): this {
        Logger.log(`Has status ${value}`);

        cy.getByTestId(`status`).should("contain.text", value);

        return this;
    }

    public hasDateTrustContacted(value: string): this {
        Logger.log(`Has date trust contacted ${value}`);

        cy.getByTestId(`date-trust-contacted`).should("contain.text", value);

        return this;
    }

    public hasReason(value: string): this {
        Logger.log(`Has reason ${value}`);

        cy.getByTestId(`reason`).should("contain.text", value);

        return this;
    }

    public hasDateAccepted(value: string): this {
        Logger.log(`Has date accepted ${value}`);

        cy.getByTestId(`date-accepted`).should("contain.text", value);

        return this;
    }

    public hasDateOfVisit(value: string): this {
        Logger.log(`Has date of visit ${value}`);

        cy.getByTestId('date-of-visit')
                .invoke('text')
                .then((text) => {
                    //remove any leading and trailing white space characters
                    const trimmedText = text.trim().replace(/\s+/g, ' ').replace(/^\s-\s/, '')
                    expect(value).to.equal(trimmedText);
                });

        return this;
    }

    public hasDateReportSentToTrust(value: string): this {
        Logger.log(`Has date report sent to trust ${value}`);

        cy.getByTestId(`date-report-sent-to-trust`).should("contain.text", value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getByTestId(`notes`).should("contain.text", value);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public resolve(): this
    {
        Logger.log("Resolving the SRMA");

        cy.getByTestId("complete-srma-button").click();

        return this;
    }

    public cancel(): this {
        Logger.log("Cancelling SRMA");

        this.getCancel().click();

        return this;
    }

    public canCancel()
    {
        Logger.log("Can cancel SRMA");

        this.getCancel();

        return this;
    }

    public cannotCancel()
    {
        Logger.log("Cannot cancel SRMA");

        this.getCancel().should("not.exist");

        return this;
    }

    public decline()
    {
        Logger.log("Declining SRMA");

        this.getDecline().click();

        return this;
    }

    public canDecline()
    {
        Logger.log("Can decline SRMA");

        this.getDecline();

        return this;
    }

    public cannotDecline()
    {
        Logger.log("Cannot decline SRMA");

        this.getDecline().should("not.exist");

        return this;
    }

    public save(): this {
        Logger.log("Saving SRMA");

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
        return cy.getByTestId("decline-srma-button");
    }
}