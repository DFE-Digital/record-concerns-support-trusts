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

    public getCaseIds(): Cypress.Chainable<Array<string>>
    {
        Logger.Log("Getting all displayed case ids");

        const result: Array<string> = [];

        return cy.containsByTestId("case-id")
        .each($el =>
        {
            result.push($el.text());
        })
        .then(() =>
        {
            return result;
        });
    }
}

const caseworkTable = new CaseworkTable();

export default caseworkTable;