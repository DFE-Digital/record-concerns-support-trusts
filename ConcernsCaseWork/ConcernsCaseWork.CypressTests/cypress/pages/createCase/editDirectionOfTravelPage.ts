import { Logger } from "../../common/logger";


export default class EditDirectionOfTravelPage {


    public hasDirectionOfTravel(value: string): this
    {
        Logger.Log(`Has direction of travel ${value}`);

        cy.getByTestId(value).should("be.checked");

        return this;
    }

    public withDirectionOfTravel(value: string): this
    {
        Logger.Log(`With direction of travel ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public apply(): this
    {
        Logger.Log("Apply direction of travel");
        cy.getByTestId("apply").click();

        return this;
    }
}