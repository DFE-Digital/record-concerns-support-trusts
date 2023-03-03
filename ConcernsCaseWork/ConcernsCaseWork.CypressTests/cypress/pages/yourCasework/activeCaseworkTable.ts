import { Logger } from "cypress/common/logger";
import { ActiveCaseRow } from "./activeCaseRow";

class ActiveCaseworkTable {
    public getRowByCaseId(caseId: string): Cypress.Chainable<ActiveCaseRow> {
        Logger.Log(`Getting the active case row for ${caseId}`);

        return cy.getByTestId(`row-${caseId}`)
        .then((el) =>
        {
            return new ActiveCaseRow(el);
        });
    }
}

const activeCaseworkTable = new ActiveCaseworkTable();

export default activeCaseworkTable;