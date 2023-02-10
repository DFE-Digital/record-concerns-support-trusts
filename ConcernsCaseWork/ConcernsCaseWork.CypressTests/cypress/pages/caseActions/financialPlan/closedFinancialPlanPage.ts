import { Logger } from "../../../common/logger";

export class ClosedFinancialPlanPage
{
    public hasStatus(value: string): this
    {
        Logger.Log(`Has status ${value}`);
        
        cy.getByTestId(`status`);
        
        return this;
    }
    
    public hasPlanRequestedDate(value: string): this
    {
        Logger.Log(`Has planRequested ${value}`);
        
        cy.getByTestId(`date-requested`);
        
        return this;
    }
    
    public hasPlanReceivedDate(value: string): this
    {
        Logger.Log(`Has planReceived ${value}`);
        
        cy.getByTestId(`date-received`);
        
        return this;
    }
    
    public hasNotes(value: string): this
    {
        Logger.Log(`Has notes ${value}`);
        
        cy.getByTestId(`notes`);
        
        return this;
    }
}