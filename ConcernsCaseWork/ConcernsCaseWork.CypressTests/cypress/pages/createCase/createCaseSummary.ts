import { Logger } from "../../common/logger";

export class CreateCaseSummary
{
    public hasTrustSummaryDetails(value: string): this
    {
        Logger.Log(`Has Trust summary details ${value}`);

        cy.getByTestId("trust-summary").should(
			"contain.text",
			value
		);
        return this;
    }

    public hasConcernType(value: string): this
    {
        Logger.Log(`Has Concern Type ${value}`);

        cy.getByTestId("concern-type").should(
			"contain.text",
			value
		);
        return this;
    }

    public hasConcernRiskRating(value: string): this
    {
        Logger.Log(`Has concern risk ${value}`);

        const riskValues = value.split(" ");

        cy.getByTestId("concern-risk-rating")
            .children()
            .each((element, index) =>
            {
                expect(element.text().trim()).to.equal(riskValues[index]);
            });

        return this;
    }

    public hasRiskToTrust(value: string): this
    {
        Logger.Log(`Has Risk to trust ${value}`);

        cy.getByTestId(`risk-to-trust`).should(
			"contain.text",
			value
		);
        
        return this;
    }

    public hasTerritory(value: string): this
    {
        Logger.Log(`Has Territory ${value}`);

        cy.getByTestId(`territory`).should(
			"contain.text",
			value
		);
        return this;
    }
}

const createCaseSummary = new CreateCaseSummary();

export default createCaseSummary; 