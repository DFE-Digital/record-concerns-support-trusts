import { Logger } from "../common/logger";

export class CookieBanner
{
    public accept() : this
    {
        Logger.log("I accept the cookie policy");
        cy.getByTestId("banner-accept-analytics-cookies-button").click();

        return this;
    }

    public reject(): this
    {
        Logger.log("I reject the cookie policy");
        cy.getByTestId("banner-reject-analytics-cookies-button").click();

        return this;
    }

    public viewCookies(): this
    {
        Logger.log("I view the cookies page");

        cy.getByTestId("view-cookies-link").click();

        return this;
    }

    public notVisible(): this
    {
        Logger.log("The cookie banner is not visible");
        cy.getByTestId("cookies-banner").should("not.exist");

        return this;
    }
}