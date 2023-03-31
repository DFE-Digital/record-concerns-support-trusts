class EditConcernPage {
    closeConcern()
    {
        cy.getByTestId("close-concern-button").click();

        return this;
    }

    confirmCloseConcern()
    {
        cy.getByTestId("close-concern-button").click();
    }
}

export default new EditConcernPage();