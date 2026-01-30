import { Logger } from '../../../common/logger';

export class DeleteSrmaPage {
    public delete(): this {
        Logger.log('Deleting of SRMA');

        cy.getByTestId('delete-srma-button').click();

        return this;
    }
}
