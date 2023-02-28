import { Logger } from "../../common/logger";

export class ViewClosedCasePage
{
    public hasConcerns(value: string): this
    {
        Logger.Log(`Has concerns ${value}`);

        cy.getByTestId(`concerns_field`);

        return this;
    }

    public hasTerritory(value: string): this
    {
        Logger.Log(`Has territory ${value}`);

        cy.getByTestId(`territory_field`);

        return this;
    }

    public hasIssue(value: string): this
    {
        Logger.Log(`Has Issue ${value}`);

        cy.getByTestId(`issue_field`);

        return this;
    }

    public hasCurrentStatus(value: string): this
    {
        Logger.Log(`Has currentStatus ${value}`);

        cy.getByTestId(`status_field`);

        return this;
    }

    public hasCaseAim(value: string): this
    {
        Logger.Log(`Has caseAim ${value}`);

        cy.getByTestId(`case_aim_field`);

        return this;
    }

    public hasDeEscalationPoint(value: string): this
    {
        Logger.Log(`Has deEscalationPoint ${value}`);

        cy.getByTestId(`deescalation_point_field`);

        return this;
    }

    public hasNextSteps(value: string): this
    {
        Logger.Log(`Has nextSteps ${value}`);

        cy.getByTestId(`next_step_field`);

        return this;
    }

    public hasCaseHistory(value: string): this
    {
        Logger.Log(`Has caseHistory ${value}`);

        cy.getByTestId(`case_history_field`);

        return this;
    }

    public hasRationaleForClosure(value: string): this
    {
        Logger.Log(`Has rationaleForClosure ${value}`);

        cy.getByTestId(`rational_for_closure_field`);

        return this;
    }

    public hasClosedCaseAction(value: string): this
    {
        Logger.Log(`Has closed case action`);

        cy.get("#close-case-actions td")
                .contains(value)


        return this;

    }

}