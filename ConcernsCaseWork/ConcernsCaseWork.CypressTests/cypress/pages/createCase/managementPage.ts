import { Logger } from "../../common/logger";


export default class CaseManagementPage {

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

}