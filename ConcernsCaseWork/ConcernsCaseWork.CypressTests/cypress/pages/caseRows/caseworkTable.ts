import { Logger } from "cypress/common/logger";
import { CaseRow } from "./caseRow";

class CaseworkTable {
    public getRowByCaseId(caseId: string): Cypress.Chainable<CaseRow> {
        Logger.log(`Getting the case row for ${caseId}`);

        return cy.getByTestId(`row-${caseId}`)
        .then((el) =>
        {
            return new CaseRow(el);
        });
    }

    public getOpenCaseIds(): Cypress.Chainable<Array<string>>
    {
        Logger.log("Getting all displayed case ids");

        const result: Array<string> = [];

        cy.getById("active-cases")
        .find(`[data-testid*='case-id']`)
        .each($el =>
        {
            result.push($el.text().trim());
        });

        return cy.wrap(result);
    }
}

const caseworkTable = new CaseworkTable();

export default caseworkTable;