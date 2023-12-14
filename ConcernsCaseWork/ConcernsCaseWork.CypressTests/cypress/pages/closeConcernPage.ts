class CloseConcernPage {
    confirmCloseConcern()
    {
        cy.getByTestId("close-concern-button").click();
    }
}

let closeConcernPage = new CloseConcernPage();

export default closeConcernPage;