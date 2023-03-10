import { EnvUrl, EnvAuthKey, CaseworkerClaim, EnvUsername } from "../constants/cypressConstants";

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
                req.headers = {
                    ...req.headers,
                    'Authorization': `Bearer ${Cypress.env(EnvAuthKey)}`,
                    "x-user-context-role-0": CaseworkerClaim,
                    "x-user-context-name": Cypress.env(EnvUsername)
                };
            }
        ).as("AuthInterceptor");
    }
}