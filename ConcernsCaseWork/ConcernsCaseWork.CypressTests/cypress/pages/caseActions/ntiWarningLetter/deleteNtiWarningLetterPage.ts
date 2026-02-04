import { Logger } from '../../../common/logger';

export class DeleteNtiWarningLetterPage {
    public delete(): this {
        Logger.log('Deleting of NTI warning letter');

        cy.getByTestId('delete-nti-wl-button').click();

        return this;
    }
}
