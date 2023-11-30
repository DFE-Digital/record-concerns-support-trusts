import { Logger } from "../../common/logger";

class AddToCasePage {

    constructor() {
    }

    public getAddToCaseBtn() {
        return cy.get('[data-prevent-double-click="true"]', { timeout: 30000 }).contains('Add to case');
    }

    public getCaseActionRadio(option: string) {
        return cy.getByTestId(option);
    }

    public addToCase(option: string) {
        this.getCaseActionRadio(option).click();
    }

    public hasActions(actions: Array<string>): this
    {
        Logger.log(`Case has available actions ${actions.join(",")}`);

        cy.get('.govuk-radios__label')
            .should("have.length", actions.length)
            .each(($elem, index) => {
                expect($elem.text().trim()).to.equal(actions[index]);
            });

        return this;
    }

    public cancel(): this
    {
        Logger.log("Cancelling case action creation");
        cy.getById("cancel-link").click();

        return this;
    }

    public hasValidationError(value: string) {
        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }
}

export default new AddToCasePage();