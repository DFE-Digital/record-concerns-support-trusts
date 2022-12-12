import { Logger } from "../../../common/logger";

export class EditNtiWarningLetterPage
{
    public save(): this
    {
        Logger.Log("Saving the NTI warning letter");

        cy.getById("add-nti-wl-button").click();

        return this;
    }
}