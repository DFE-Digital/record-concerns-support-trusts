import { Logger } from "../../../common/logger";

export class ViewNtiUnderConsiderationPage
{
    public close(): this
    {
        Logger.Log("Closing NTI under consideration");

        cy.getByTestId("close-nti-uc-button").click();

        return this;
    }
}