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

    public hasConcernRiskRating(riskValues: string[]): this
    {
        Logger.Log(`Has concern risk ${riskValues}`);

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

    public hasManagedBy(division: string, territory: string): this
    {
        Logger.Log(`Has Managed By ${division} ${territory}`);

        cy.getByTestId(`managed-by`).should(
			"contain.text",
			division
		);

        cy.getByTestId(`managed-by`).should(
			"contain.text",
			territory
		);

        return this;
    }

    public hasHintText(hintText: string): this
    {
        Logger.Log(`Has Hint Text By ${hintText} `);

        cy.getByTestId(`hint-text`).should(
			"contain.text",
			hintText
		);

        return this;
    }
}

const createCaseSummary = new CreateCaseSummary();

export default createCaseSummary; 