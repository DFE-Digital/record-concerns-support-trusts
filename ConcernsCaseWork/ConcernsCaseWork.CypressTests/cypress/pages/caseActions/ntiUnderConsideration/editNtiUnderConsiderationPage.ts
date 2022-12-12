import { Logger } from "../../../common/logger";

export class EditNtiUnderConsiderationPage
{
    public save(): this
    {
        Logger.Log("Saving NTI under consideration");

        cy.getById("add-nti-uc-button").click();

        return this;
    }
}