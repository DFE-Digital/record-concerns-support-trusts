import { Logger } from "../../common/logger";


class EditRegionPage {

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

    public apply(): this
    {
        Logger.Log("Apply changes");
        cy.getByTestId("save-case").click();

        return this;
    }
}

const editRegionPage = new EditRegionPage();

export default editRegionPage;