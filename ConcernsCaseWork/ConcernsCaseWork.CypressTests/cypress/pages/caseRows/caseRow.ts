import { Logger } from "cypress/common/logger";

export class CaseRow {
    constructor(private element: Element)
    {

    }

    public hasCaseId(value: string) {
        Logger.Log(`Has case id ${value}`);

        this.containsText("case-id", value);

        return this;
    }

    public hasCreatedDate(value: string): this {
        Logger.Log(`Has created date ${value}`);

        this.containsText("created-at", value);

        return this;
    }

    public hasClosedDate(value: string): this {
        Logger.Log(`Has closed date ${value}`);

        this.containsText("closed-at", value);

        return this;
    }

    public hasTrust(value: string): this {
        Logger.Log(`Has trust name ${value}`);
        
        this.containsText("trust-name", value);

        return this;
    }

    public hasConcern(value: string): this {
        Logger.Log(`Has concern ${value}`);

        this.containsText("concern", value);

        return this;
    }

    public hasRiskToTrust(value: string): this {
        Logger.Log(`Has risk to trust ${value}`);

        this.containsText("risk-to-trust-list", value);

        return this;
    }

    public hasManagedBy(division: string, area: string): this
    {
        Logger.Log(`Has managed by ${division} ${area}`);

        cy.getByTestId("managed-by").should("contain.text", division);
        cy.getByTestId("managed-by").should("contain.text", area);

        return this;
    }

    public hasAction(value: string): this
    {
        Logger.Log(`Has action ${value}`);

        cy.getByTestId("actions-and-decisions").should("contain.text", value);

        return this;
    }

    public hasOwner(value: string): this
    {
        Logger.Log(`Has owner ${value}`);
        
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
        Logger.Log(`Selecting case`);

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