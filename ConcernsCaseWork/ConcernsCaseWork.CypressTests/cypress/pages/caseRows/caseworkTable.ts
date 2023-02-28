import { Logger } from "cypress/common/logger";
import { CaseRow } from "./caseRow";

class CaseworkTable {
    public getRowByCaseId(caseId: string): Cypress.Chainable<CaseRow> {
        Logger.Log(`Getting the case row for ${caseId}`);

        return cy.getByTestId(`row-${caseId}`)
        .then((el) =>
        {
            return new CaseRow(el);
        });
    }

    public selectCase(caseId: string) {
        Logger.Log(`Selecting case ${caseId}`);

        return cy.get('a[href="/case/'+caseId+'/closed"]')
        .click();
    }
}

const caseworkTable = new CaseworkTable();

export default caseworkTable;