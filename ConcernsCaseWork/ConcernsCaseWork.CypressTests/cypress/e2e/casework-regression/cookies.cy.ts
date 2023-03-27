import { Logger } from "../../common/logger";
import { CookieBanner } from "../../pages/cookieBanner";
import { CookiesPage } from "../../pages/cookiesPage";

describe("Testing cookies on the site", () => 
{
    let cookiesPage = new CookiesPage();
    let cookiesBanner = new CookieBanner();

    beforeEach(() =>
    {
        cy.login();
    });

    it("Should accept the cookies on the banner then decline them afterwards", () =>
    {
        cy.visit("/trust");

        cookiesBanner
            .accept()
            .notVisible();

        Logger.Log("Upon accepting the banner it should stay on the same page");
        cy.url().should("include", "/trust");

        cy.visit("/cookies");

        cookiesPage
            .hasConsent("Yes")

        hasCookieValue("True");

        cookiesPage
            .withConsent("No")
            .save();

        cy.reload();

        cookiesPage.hasConsent("No");

        hasCookieValue("False");
    });

    it("Should reject the cookies on the banner then accept them afterwards", () =>
    {
        cookiesBanner.viewCookies();

        cy.url().should("include", "/cookies");

        cookiesBanner
            .reject()
            .notVisible();

        cookiesPage
        .hasConsent("No")

        hasCookieValue("False");

        cookiesPage
        .withConsent("Yes")
        .save();

        cy.reload();

        cookiesPage.hasConsent("Yes");

        hasCookieValue("True");
    });

    function hasCookieValue(cookieValue: string)
    {
        Logger.Log(`Should set the consent cookie to ${cookieValue}`);

        cy.getCookie(".ConcernsCasework.Consent")
        .then(cookie =>
        {
            expect(cookie?.value).to.equal(cookieValue);
        });
    }
});