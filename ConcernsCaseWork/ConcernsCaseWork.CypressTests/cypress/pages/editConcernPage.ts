class EditConcernPage {
    confirmCloseConcern()
    {
        cy.getByTestId("close-concern-button").click();
    }
}

export default new EditConcernPage();