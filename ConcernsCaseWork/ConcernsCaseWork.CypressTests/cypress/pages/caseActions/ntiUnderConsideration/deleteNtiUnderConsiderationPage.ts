import { Logger } from "../../../common/logger";

export class DeleteNtiUnderConsiderationPage
{
    public delete(): this
    {
        Logger.log("Deleting the NTI under consideration");

        cy.getByTestId("delete-nti-uc-button").click();

        return this;
    }
}
