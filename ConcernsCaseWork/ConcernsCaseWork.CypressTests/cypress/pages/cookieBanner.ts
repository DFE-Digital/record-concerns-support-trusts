import { Logger } from "../common/logger";

export class CookieBanner
{
    public accept() : this
    {
        Logger.Log("I accept the cookie policy");
        cy.getByTestId("banner-accept-analytics-cookies-button").click();

        return this;
    }

    public reject(): this
    {
        Logger.Log("I reject the cookie policy");
        cy.getByTestId("banner-reject-analytics-cookies-button").click();

        return this;
    }

    public viewCookies(): this
    {
        Logger.Log("I view the cookies page");

        cy.getByTestId("view-cookies-link").click();

        return this;
    }

    public notVisible(): this
    {
        Logger.Log("The cookie banner is not visible");
        cy.getByTestId("cookies-banner").should("not.exist");

        return this;
    }
}