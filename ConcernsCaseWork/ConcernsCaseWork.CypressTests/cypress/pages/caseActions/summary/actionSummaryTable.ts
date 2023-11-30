import { Logger } from "cypress/common/logger";
import { ActionSummaryRow } from "./actionSummaryRow";

export class ActionSummaryTable {
    public getOpenAction(value: string): Cypress.Chainable<ActionSummaryRow> {
        Logger.log(`Getting open action summary row ${value}`);

        return cy.getById("open-case-actions")
            .find(`[data-testid='row-${value}']`)
            .then((el) =>
            {
                return new ActionSummaryRow(el);
            });
    }

    public getClosedAction(value: string): Cypress.Chainable<ActionSummaryRow> {
        Logger.log(`Getting closed action summary row ${value}`);

        return cy.getById("close-case-actions")
            .find(`[data-testid='row-${value}']`)
            .then((el) =>
            {
                return new ActionSummaryRow(el);
            });
    }
}

const actionSummaryTable = new ActionSummaryTable();

export default actionSummaryTable;