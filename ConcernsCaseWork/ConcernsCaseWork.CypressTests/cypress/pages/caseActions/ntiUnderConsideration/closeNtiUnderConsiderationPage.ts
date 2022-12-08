import { Logger } from "../../../common/logger";

export class CloseNtiUnderConsiderationPage
{
    public withReason(reason: string): this
    {
        Logger.Log(`With reason for closure ${reason}`);

        cy.getByTestId(reason).click();

        return this;
    }

    public close(): this
    {
        Logger.Log("Closing the NTI under consideration");

        cy.getById("add-nti-uc-button").click();

        return this;
    }
}