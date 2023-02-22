import { Logger } from "cypress/common/logger";

export class ActionSummaryRow {

    constructor(private element: Element) 
    {
    }

    public hasName(value: string): this {
        Logger.Log(`Has name ${value}`);

        cy.wrap(this.element).getByTestId(value).should("contain.text", value);

        return this;
    }

    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        cy.wrap(this.element).getByTestId("status").should("contain.text", value);

        return this;
    }

    public hasCreatedDate(value: string): this {
        Logger.Log(`Has created date ${value}`);

        cy.wrap(this.element).getByTestId("opened-date").should("contain.text", value);

        return this;
    }

    public hasClosedDate(value: string): this {
        Logger.Log(`Has closed date ${value}`);

        cy.wrap(this.element).getByTestId("closed-date").should("contain.text", value);

        return this;
    }

    public select(): this {
        Logger.Log(`Selecting action`);

        cy.wrap(this.element)
        .within(() =>
        {
            cy.get('a').click();
        })

        return this;
    }
}

