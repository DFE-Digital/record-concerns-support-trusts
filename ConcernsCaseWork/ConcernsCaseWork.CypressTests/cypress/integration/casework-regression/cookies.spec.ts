import { Logger } from "../../common/logger";
import { CookiesPage } from "../../pages/cookiesPage";

describe("Testing cookies on the site", () => 
{
    let cookiesPage = new CookiesPage();

    beforeEach(() =>
    {
        cy.login();
    });

    describe("When accepting the cookies for the site", () =>
    {
        it("Should set the consent cookie to true", () =>
        {
            cy.visit("/cookies");

            cookiesPage
                .withConsent("Yes")
                .save();

            cy.visit("/");
            cy.visit("/cookies");

            cookiesPage.hasConsent("Yes");

            Logger.Log("Should set the consent cookie to True");

            cy.getCookie(".ConcernsCasework.Consent")
                .then(cookie =>
                {
                    expect(cookie?.value).to.equal("True");
                });
        });
    });

    describe("When rejecting the cookies for the site", () =>
    {
        it("Should set the consent cookie to false", () =>
        {
            cy.visit("/cookies");

            cookiesPage
                .withConsent("No")
                .save();

            Logger.Log("Should set the consent cookie to No");

            cy.getCookie(".ConcernsCasework.Consent")
                .then(cookie =>
                {
                    expect(cookie?.value).to.equal("False");
                });
        });
    });
});