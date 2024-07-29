class DeleteConcernPage {
    confirmDeleteConcern()
    {
        cy.getByTestId("delete-concern-button").click();
    }
}

let deleteConcernPage = new DeleteConcernPage();

export default deleteConcernPage;