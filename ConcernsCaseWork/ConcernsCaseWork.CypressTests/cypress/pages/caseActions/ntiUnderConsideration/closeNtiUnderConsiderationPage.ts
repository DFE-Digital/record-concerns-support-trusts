import { Logger } from "../../../common/logger";

export class CloseNtiUnderConsiderationPage
{
    public withStatus(status: string): this
    {
        Logger.Log(`With closure status ${status}`);

        cy.getByTestId(status).click();

        return this;
    }

    public withNotes(value: string): this {
        Logger.Log(`With notes ${value}`);

        cy.getById(`nti-notes`).clear().type(value);

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getById(`nti-notes`).should("contain.value", value);

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public close(): this
    {
        Logger.Log("Closing the NTI under consideration");

        cy.getById("add-nti-uc-button").click();

        return this;
    }
}