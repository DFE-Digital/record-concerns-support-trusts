import { Logger } from "cypress/common/logger";

export class ActionSummaryRow {

    constructor(private element: Element) 
    {
    }

    public hasName(value: string): this {
        Logger.log(`Has name ${value}`);

        cy.wrap(this.element)
        .within(() => 
        {
            cy.getByTestId(value).should("contain.text", value);
        })

        return this;
    }

    public hasStatus(value: string): this {
        Logger.log(`Has status ${value}`);

        cy.wrap(this.element)
        .within(() => {
            cy.getByTestId("status").should("contain.text", value);
        })

        return this;
    }

    public hasCreatedDate(value: string): this {
        Logger.log(`Has created date ${value}`);

        cy.wrap(this.element)
        .within(() => {
            cy.getByTestId("opened-date").should("contain.text", value);
        })

        return this;
    }

    public hasClosedDate(value: string): this {
        Logger.log(`Has closed date ${value}`);

        cy.wrap(this.element)
        .within(() => {
            cy.getByTestId("closed-date").should("contain.text", value);
        })

        return this;
    }

    public select(): this {
        Logger.log(`Selecting action`);

        cy.wrap(this.element)
        .within(() =>
        {
            cy.get('a').click();
        })

        return this;
    }
}

