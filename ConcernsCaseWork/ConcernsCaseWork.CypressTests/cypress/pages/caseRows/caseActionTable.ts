import { Logger } from "cypress/common/logger";
import { ActionRow } from "./actionRow";

class CaseActionTable {
    public getRowByAction(action: string): Cypress.Chainable<ActionRow> {
        Logger.Log(`Getting the case action row for ${action}`);

        return cy.getByTestId(`row-${action}`)
        .then((el) =>
        {
            return new ActionRow(el);
        });
    }
}

const caseActionTable = new CaseActionTable();

export default caseActionTable;