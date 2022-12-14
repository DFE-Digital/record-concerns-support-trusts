import { Logger } from "../common/logger";

export class CookiesPage
{
    public withConsent(consent: string): this
    {
        Logger.Log(`With cookie consent value ${consent}`);

        cy.getByTestId(`analytics-cookies-${consent.toLowerCase()}`).check();

        return this;
    }

    public hasConsent(consent): this
    {
        Logger.Log(`Has cookie consent value ${consent}`);

        cy.getByTestId(`analytics-cookies-${consent.toLowerCase()}`).should("be.checked");

        return this;
    }

    public save(): this
    {
        Logger.Log("Saving cookie consent");

        cy.getByTestId("save-cookie-preferences-button").click();

        return this;
    }
}