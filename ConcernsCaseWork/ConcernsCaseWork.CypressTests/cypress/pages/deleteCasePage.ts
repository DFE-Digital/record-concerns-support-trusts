import { Logger } from "cypress/common/logger";

class DeleteCasePage {
    confirmDeleteCase()
    {
        cy.getById("delete-case-button").click();
    }

    public withRationaleForClosure(reason: string): this {
		Logger.log(`With rationale for closure ${reason}`);
		cy.getById("case-outcomes").clear().type(reason);

		return this;
	}
}

let deleteCasePage = new DeleteCasePage();

export default deleteCasePage;
    