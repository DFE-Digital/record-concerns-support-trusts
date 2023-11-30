import { Logger } from "../common/logger";

export class CookiesPage
{
    public withConsent(consent: string): this
    {
        Logger.log(`With cookie consent value ${consent}`);

        cy.getByTestId(`analytics-cookies-${consent.toLowerCase()}`).check();

        return this;
    }

    public hasConsent(consent): this
    {
        Logger.log(`Has cookie consent value ${consent}`);

        cy.getByTestId(`analytics-cookies-${consent.toLowerCase()}`).should("be.checked");

        return this;
    }

    public save(): this
    {
        Logger.log("Saving cookie consent");

        cy.getByTestId("save-cookie-preferences-button").click();

        return this;
    }
}