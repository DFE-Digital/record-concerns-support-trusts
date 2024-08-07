import { Logger } from "cypress/common/logger";

class DeleteCasePage {
    confirmDeleteCase()
    {
        cy.getById("delete-case-button").click();
    }
}

let deleteCasePage = new DeleteCasePage();

export default deleteCasePage;
    