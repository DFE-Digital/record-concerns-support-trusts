import { Logger } from "../../../common/logger";

export class CloseNoticeToImprovePage
{
    public close(): this
    {
        Logger.Log("Confirming Close of Notice To Improve");

        cy.getById("add-nti-wl-button").click();

        return this;
    }
}