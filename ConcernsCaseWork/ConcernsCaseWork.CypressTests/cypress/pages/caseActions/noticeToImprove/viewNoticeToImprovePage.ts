import { Logger } from "../../../common/logger";

export class ViewNoticeToImprovePage
{
    public close(): this
    {
        Logger.Log("Closing Notice To Improve");

        cy.getByTestId("close-nti-button").click();

        return this;
    }
}