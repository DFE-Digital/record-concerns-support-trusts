import { Logger } from "../../common/logger";


class AddRegionPage {

    public hasRegion(value: string): this
    {
        Logger.log(`Has region ${value}`);

        cy.getByTestId(value).should("be.checked")

        return this;
    }

    public withRegion(value: string): this
    {
        Logger.log(`With Region ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public nextStep(): this
    {
        Logger.log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }
}

const addRegionPage = new AddRegionPage();

export default addRegionPage;