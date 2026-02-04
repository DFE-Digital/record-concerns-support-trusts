import { Logger } from '../../../common/logger';

export class ViewTargetedTrustEngagementPage {
    public hasDateOpened(value: string): this {
        Logger.log(`Has date opened ${value}`);

        cy.getByTestId('engagement-open-text').should('contain.text', value);

        return this;
    }

    public hasDateClosed(value: string): this {
        Logger.log(`Has date closed ${value}`);

        cy.getByTestId('engagement-closed-text').should('contain.text', value);

        return this;
    }

    public hasDateBegan(value: string): this {
        Logger.log(`Has date opened ${value}`);

        cy.getByTestId('engagement-began-text').should('contain.text', value);

        return this;
    }

    public hasDateEnded(value: string): this {
        Logger.log(`Has date opened ${value}`);

        cy.getByTestId('engagement-ended-text').should('contain.text', value);

        return this;
    }

    public hasTypeOfEngagement(typeOfEngagement: string): this {
        cy.task('log', `Has type of engagement  ${typeOfEngagement}`);

        cy.getByTestId('engagement-type-text').should('contain.text', typeOfEngagement);

        return this;
    }

    public hasSupportingNotes(supportingNotes: string): this {
        cy.task('log', `Has Supporting Notes ${supportingNotes}`);

        cy.getByTestId('supporting-notes-text').should('contain.text', supportingNotes);

        return this;
    }

    public hasActionEdit(): this {
        cy.task('log', `Has Edit Action`);

        cy.getByTestId('edit-engagement-text').should('contain.text', 'Edit');

        return this;
    }

    public hasAuthoriser(authoriser: string): this {
        Logger.log(`Has authoriser ${authoriser}`);

        cy.getByTestId('authoriser-text').should('contain.text', authoriser);

        return this;
    }

    public editTTE(): this {
        Logger.log('Editing TTE');

        this.getEditTTE().children('a').click();

        return this;
    }

    public deleteTTE(): this {
        Logger.log('Delete TTE');

        this.getDeleteTTE().click();

        return this;
    }

    public canEditTTE(): this {
        Logger.log('Can edit TTE');

        this.getEditTTE();

        return this;
    }

    public cannotEditTTE(): this {
        Logger.log('Cannot edit TTE');

        this.getEditTTE().should('not.exist');

        return this;
    }

    public closeTTE(): this {
        Logger.log('Closing TTE');
        this.getCloseTTE().click();

        return this;
    }

    public cannotCloseTTE(): this {
        Logger.log('Cannot close TTE');

        this.getCloseTTE().should('not.exist');

        return this;
    }

    private getEditTTE() {
        return cy.getByTestId('edit-engagement-text');
    }

    private getDeleteTTE() {
        return cy.getByTestId('delete-engagement');
    }

    private getCloseTTE() {
        return cy.getByTestId('close-engagement-button');
    }
}
