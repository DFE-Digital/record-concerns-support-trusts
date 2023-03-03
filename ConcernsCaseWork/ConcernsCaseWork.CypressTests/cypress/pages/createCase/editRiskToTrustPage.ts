import { Logger } from "../../common/logger";


export default class EditRiskToTrustPage {


    public hasRiskToTrust(value: string): this
    {
        Logger.Log(`Has risk to trust ${value}`);

        cy.getByTestId(value).should("be.checked");

        return this;
    }

    public withRiskToTrust(value: string): this
    {
        Logger.Log(`With risk to trust ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public apply(): this
    {
        Logger.Log("Apply risk to trust");
        cy.getByTestId("apply").click();

        return this;
    }
}