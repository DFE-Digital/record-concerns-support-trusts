import { GlobalCache } from "../support/globalCache";

export class AuthenticationComponent {

    constructor()
    {
        this.globalCache = new GlobalCache();
    }

    login(username, password) {

        this.globalCache.getItem(username).then(value => {
            if (value) {
                this.setCookie(value);
            }
            else {
                this.cacheAndSetCookie(username, password);
            }
        })
    }

    cacheAndSetCookie(username, password) {
        cy.task('azureLogin', {url: Cypress.env('url'), username: username, password: password})
            .then((cookie) => {
                this.globalCache.setItem(username, cookie);
                this.setCookie(cookie);
            });
    }

    setCookie(cookie) {
        cy.setCookie('.ConcernsCasework.Login', cookie);
    }
}