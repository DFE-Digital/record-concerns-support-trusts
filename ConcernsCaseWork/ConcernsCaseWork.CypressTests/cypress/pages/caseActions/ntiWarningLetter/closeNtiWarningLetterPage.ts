import { Logger } from "../../../common/logger";

export class CloseNtiWarningLetterPage
{
    public withReason(reason: string): this
    {
        Logger.Log(`With reason ${reason}`);

        cy.getByTestId(reason).click();

        return this;
    }

    public close(): this
    {
        Logger.Log("Confirmingh close of NTI warning letter");

        cy.getById("add-nti-wl-button").click();

        return this;
    }
}