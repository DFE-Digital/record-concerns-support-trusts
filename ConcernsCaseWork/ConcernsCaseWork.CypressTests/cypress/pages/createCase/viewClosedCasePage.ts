import { Logger } from "../../common/logger";

export class ViewClosedCasePage
{
    public hasConcerns(value: string): this
    {
        Logger.log(`Has concerns ${value}`);

        cy.getByTestId(`concerns_field`).should(`contain.text`, value);

        return this;
    }

    public hasTrust(value: string): this
    {
        Logger.log(`Has trust ${value}`);

        cy.getByTestId("trust-name").should("contain.text", value);

        return this;
    }

    public hasManagedBy(division: string, territory: string): this
    {
        Logger.log(`Has managed by ${division} ${territory}`);

        cy.getByTestId(`territory_field`).should(`contain.text`, division);
        cy.getByTestId(`territory_field`).should(`contain.text`, territory);

        return this;
    }

    public hasCaseOwner(value: string): this
    {
        Logger.log(`Has case owner ${value}`);

        cy.getByTestId("case-owner-field").contains(value, { matchCase: false });

        return this;
    }

    public hasDateCreated(value: string): this
    {
        Logger.log(`Has date created ${value}`);

        cy.getByTestId("date-created-field").should("contain.text", value);

        return this;
    }

    public hasDateClosed(value: string): this
    {
        Logger.log(`Has date closed ${value}`);

        cy.getByTestId("date-closed-field").should("contain.text", value);

        return this;
    }

    public hasNoCaseNarritiveFields(): this
    {
        Logger.log("Has no case narritive fields");

        this.getIssue().should("not.exist");
        this.getCurrentStatus().should("not.exist");
        this.getCaseAim().should("not.exist");
        this.getDeEscalationPoint().should("not.exist");
        this.getNextSteps().should("not.exist");
        this.getCaseHistory().should("not.exist");

        return this;
    }

    public hasIssue(value: string): this
    {
        Logger.log(`Has Issue ${value}`);

        this.getIssue().should(`contain.text`, value);

        return this;
    }

    public hasCurrentStatus(value: string): this
    {
        Logger.log(`Has currentStatus ${value}`);

        this.getCurrentStatus().should(`contain.text`, value);

        return this;
    }

    public hasCaseAim(value: string): this
    {
        Logger.log(`Has caseAim ${value}`);

        this.getCaseAim().should(`contain.text`, value);

        return this;
    }

    public hasDeEscalationPoint(value: string): this
    {
        Logger.log(`Has deEscalationPoint ${value}`);

        this.getDeEscalationPoint().should(`contain.text`, value);

        return this;
    }

    public hasNextSteps(value: string): this
    {
        Logger.log(`Has nextSteps ${value}`);

        this.getNextSteps().should(`contain.text`, value);

        return this;
    }

    public hasCaseHistory(value: string): this
    {
        Logger.log(`Has caseHistory ${value}`);

        this.getCaseHistory().should(`contain.text`, value);

        return this;
    }

    public hasRationaleForClosure(value: string): this
    {
        Logger.log(`Has rationaleForClosure ${value}`);

        cy.getByTestId(`rational_for_closure_field`).should(`contain.text`, value);

        return this;
    }

    public hasClosedCaseAction(value: string): this
    {
        Logger.log(`Has closed case action`);

        cy.get("#close-case-actions td")
        .should(`contain.text`, value)


        return this;

    }

    private getIssue() {
        return cy.getByTestId(`issue_field`);
    }

    private getCurrentStatus() {
        return cy.getByTestId(`status_field`);
    }

    private getCaseAim() {
        return cy.getByTestId(`case_aim_field`);
    }

    private getDeEscalationPoint() {
        return cy.getByTestId(`deescalation_point_field`);
    }

    private getNextSteps() {
        return cy.getByTestId(`next_step_field`);
    }

    private getCaseHistory() {
        return cy.getByTestId(`case_history_field`);
    }
}