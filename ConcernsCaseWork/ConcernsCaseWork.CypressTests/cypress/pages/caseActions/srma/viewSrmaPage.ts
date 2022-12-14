import { Logger } from "../../../common/logger";

export class ViewSrmaPage
{
    public cancelSrma(): this
    {
        Logger.Log("Cancelling SRMA");

        cy.getByTestId("cancel-srma-button").click();

        return this;
    }

    public addReason(): this
    {
        Logger.Log("Adding Srma reason");
        cy.getByTestId("SRMA reason").click();

        return this;
    }

    public withSrmaReason(reason: string): this
    {
        Logger.Log(`With SRMA reason`);

        const id = reason.split(" ").join("");

        cy.getByTestId(id).click();

        return this;
    }

    public save(): this
    {
        Logger.Log("Saving SRMA");

        cy.getById("add-srma-button").click();

        return this;
    }

    public confirmCancellation(): this
    {
        Logger.Log("Confirming the SRMA is cancelled");

        cy.getById("confirmChk").check();

        return this;
    }
}