import { Logger } from '../../../common/logger';

export class DeleteTrustFinancialForecastPage {
    public delete(): this {
        Logger.log('Deleting the Trust financial forecast');

        cy.getByTestId('delete-tff-button').click();

        return this;
    }
}
