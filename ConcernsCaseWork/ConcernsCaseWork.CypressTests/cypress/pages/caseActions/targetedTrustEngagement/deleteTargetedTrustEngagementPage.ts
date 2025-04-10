import { Logger } from "../../../common/logger";

export class DeleteTargetedTrustEngagementPage
{
    public delete(): this {
        Logger.log("Delete the TTE");

        cy.getByTestId('delete-tte-button').click();

        return this;
    }
}
