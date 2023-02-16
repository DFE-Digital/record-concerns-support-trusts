import { Logger } from "../../common/logger";


export default class CaseManagementPage {


    public hasTrust(value: string): this
    {
        Logger.Log(`Has trust ${value}`);

        cy.getByTestId(`trust_Field`).should("contain.text", value);

        return this;
    }

    public hasRiskToTrust(value: string): this
    {
        Logger.Log(`Has risk to trust ${value}`);

        cy.getByTestId(`risk_to_trust_Field`).should("contain.text", value);

        return this;
    }

    public hasConcerns(value: string): this
    {
        Logger.Log(`Has concerns ${value}`);

        cy.getByTestId(`concerns_Field`).should("contain.text", value);

        return this;
    }

    public hasTerritory(value: string): this
    {
        Logger.Log(`Has territort ${value}`);

        cy.getByTestId(`territory_Field`).should("contain.text", value);

        return this;
    }

    public hasIssue(value: string): this
    {
        Logger.Log(`Has issue ${value}`);

        cy.getByTestId(`issue`).should("contain.text", value);

        return this;
    }

    public hasCurrentStatus(value: string): this
    {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`status`).should("contain.text", value);

        return this;
    }

    public hasCaseAim(value: string): this
    {
        Logger.Log(`Has caseAim ${value}`);

        cy.getByTestId(`case-aim`).should("contain.text", value);

        return this;
    }

    public hasDeEscalationPoint(value: string): this
    {
        Logger.Log(`Has deEscalationPoint ${value}`);

        cy.getByTestId(`de-escalation-point`).should("contain.text", value);

        return this;
    }

    public hasNextSteps(value: string): this
    {
        Logger.Log(`Has nextSteps ${value}`);

        cy.getByTestId(`next-steps`).should("contain.text", value);

        return this;
    }

    public hasCaseHistory(value: string): this
    {
        Logger.Log(`Has caseHistory ${value}`);

        cy.getByTestId(`case-history`).should("contain.text", value);

        return this;
    }

    public hasEmptyCurrentStatus(): this
    {
        this.hasCurrentStatus("");

        return this;
    }

    public hasEmptyCaseAim(): this
    {

        this.hasCaseAim("");

        return this;
    }

    public hasEmptyDeEscalationPoint(): this
    {
        this.hasDeEscalationPoint("");

        return this;
    }

    public hasEmptyNextSteps(): this
    {

        this.hasNextSteps("");

        return this;
    }

    public hasEmptyCaseHistory(): this
    {
        this.hasCaseHistory("");

        return this;
    }
}