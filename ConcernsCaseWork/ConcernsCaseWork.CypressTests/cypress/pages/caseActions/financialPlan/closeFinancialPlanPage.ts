import { Logger } from "../../../common/logger";

export class CloseFinancialPlanPage
{
    public withReasonForClosure(reason: string): this
    {
        Logger.Log(`With reason for closure ${reason}`);

        const id = reason.split(" ").join("");

        cy.getByTestId(id).click();

        return this;
    }

    public close()
    {
        Logger.Log("Confirming the closure of the financial plan");

        cy.getById("add-financial-plan-button").click();

        return this;
    }
}