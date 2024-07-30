import { Logger } from "../../../common/logger";

export class DeleteFinancialPlanPage
{
	public delete(): this {
		Logger.log("Delete the financial plan");

		cy.getByTestId('delete-financial-plan-button').click();

		return this;
	}
}