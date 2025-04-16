import { Logger } from "../../../common/logger";

export class DeleteDecisionPage
{
    public delete(): this {
        Logger.log("Delete the decision");

        cy.getByTestId('delete-decision-button').click();

        return this;
    }
}
