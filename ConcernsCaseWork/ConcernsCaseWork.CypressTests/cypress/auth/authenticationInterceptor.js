import { EnvUrl, EnvAuthKey, CaseworkerClaim, EnvUsername, EnvApiKey } from "../constants/cypressConstants";

export class AuthenticationInterceptor {

    register() {
        cy.intercept(
            {
                url: Cypress.env(EnvUrl) + "/**",
                middleware: true,
            },
            (req) =>
            {
                // Set an auth header on every request made by the browser
                req.headers['Authorization'] = `Bearer ${Cypress.env(EnvAuthKey)}`;
                req.headers = {
                    ...req.headers,
                    "ApiKey": Cypress.env(EnvApiKey),
                    "x-user-context-role-0" : CaseworkerClaim,
                    "x-user-context-name" : Cypress.env(EnvUsername)
                };
            }
        ).as("AuthInterceptor");
    }
}