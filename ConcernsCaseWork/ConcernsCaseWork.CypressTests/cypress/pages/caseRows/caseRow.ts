import { Logger } from "cypress/common/logger";

export class CaseRow {
    constructor(private element: Element)
    {

    }

    public hasCaseId(value: string) {
        Logger.log(`Has case id ${value}`);

        this.containsText("case-id", value);

        return this;
    }

    public hasCreatedDate(value: string): this {
        Logger.log(`Has created date ${value}`);

        this.containsText("created-at", value);

        return this;
    }

    public hasLastUpdatedDate(value: string): this {
        Logger.log(`Has last updated date ${value}`);

        this.containsText("updated-at", value);

        return this;
    }

    public hasClosedDate(value: string): this {
        Logger.log(`Has closed date ${value}`);

        this.containsText("closed-at", value);

        return this;
    }

    public hasTrust(value: string): this {
        Logger.log(`Has trust name ${value}`);
        
        this.containsText("trust-name", value);

        return this;
    }

    public hasConcern(value: string): this {
        Logger.log(`Has concern ${value}`);

        this.containsText("concern", value);

        return this;
    }

    public hasRiskToTrust(value: string): this {
        Logger.log(`Has risk to trust ${value}`);

        this.containsText("risk-to-trust-list", value);

        return this;
    }

    public hasManagedBy(division: string, area: string): this
    {
        Logger.log(`Has managed by ${division} ${area}`);

        cy.getByTestId("managed-by").should("contain.text", division);
        cy.getByTestId("managed-by").should("contain.text", area);

        return this;
    }

    public hasAction(value: string): this
    {
        Logger.log(`Has action ${value}`);

        cy.getByTestId("actions-and-decisions").should("contain.text", value);

        return this;
    }

    public hasOwner(value: string): this
    {
        Logger.log(`Has owner ${value}`);
        
        cy.wrap(this.element)
        .within(() => 
        {
            cy.getByTestId("created-by").should(($element: any) =>
            {
                expect($element.text().toLowerCase()).to.include(value.toLocaleLowerCase());
            });
        })

        return this;
    }

    public select(): this {
        Logger.log(`Selecting case`);

        cy.wrap(this.element)
        .within(() =>
        {
            cy.get('a').click();
        })

        return this;
    }

    private containsText(id: string, value: string) {
        cy.wrap(this.element)
        .within(() => 
        {
            cy.getByTestId(id).should("contain.text", value);
        })

    }
}