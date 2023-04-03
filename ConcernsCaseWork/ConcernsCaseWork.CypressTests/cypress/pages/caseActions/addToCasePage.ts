class AddToCasePage {

    constructor() {
    }

    getAddToCaseBtn() {
        return cy.get('[data-prevent-double-click="true"]', { timeout: 30000 }).contains('Add to case');
    }

    getCaseActionRadio(option: string) {
        return cy.get('[value="' + option + '"]');
    }

    addToCase(option: string) {
        this.getCaseActionRadio(option).click();
    }

    hasValidationError(value: string) {
        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }
}

export default new AddToCasePage();