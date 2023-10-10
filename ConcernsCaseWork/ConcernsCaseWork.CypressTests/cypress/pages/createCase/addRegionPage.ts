import { Logger } from "../../common/logger";


class AddRegionPage {

    public hasRegion(value: string): this
    {
        Logger.Log(`Has region ${value}`);

        cy.getByTestId(value).should("be.checked")

        return this;
    }

    public withRegion(value: string): this
    {
        Logger.Log(`With Region ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public nextStep(): this
    {
        Logger.Log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }
}

const addRegionPage = new AddRegionPage();

export default addRegionPage;