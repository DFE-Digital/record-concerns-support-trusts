import { Logger } from "../../../common/logger";

export class EditNoticeToImprovePage
{
    public save(): this
    {
        Logger.Log("Saving Notice To Improve");

        cy.getById("add-nti-wl-button").click();

        return this;
    }
}