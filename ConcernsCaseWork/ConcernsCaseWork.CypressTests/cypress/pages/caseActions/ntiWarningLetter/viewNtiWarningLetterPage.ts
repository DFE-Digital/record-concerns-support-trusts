import { Logger } from "../../../common/logger";

export class ViewNtiWarningLetterPage
{
    public close(): this
    {
        Logger.Log("Closing NTI warning letter");

        cy.getByTestId("close-nti-wl-button").click();

        return this;
    }
}