import { Logger } from "cypress/common/logger";

export class ActionRow {
    constructor(private element: Element)
    {

    }

    public hasCreatedDate(value: string): this {
        Logger.Log(`Has created date ${value}`);

        this.containsText("created-at", value);

        return this;
    }

    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        this.containsText("status", value);

        return this;
    }

    public hasOpenedDate(value: string): this {
        Logger.Log(`Has opened date ${value}`);
        
        this.containsText("opened-date", value);

        return this;
    }

    public hasClosedDate(value: string): this {
        Logger.Log(`Has closed date ${value}`);
        
        this.containsText("closed-date", value);

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