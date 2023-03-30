import { EnvUrl, EnvAuthKey, CaseworkerClaim, EnvUsername } from "../constants/cypressConstants";

export class AuthenticationInterceptor {

    register(params?: AuthenticationInterceptorParams) {
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
                    "x-user-context-role-0": params?.role ? params.role : CaseworkerClaim,
                    "x-user-context-name": params?.username ? params.username : Cypress.env(EnvUsername)
                };
            }
        ).as("AuthInterceptor");
    }
}

export type AuthenticationInterceptorParams = {
    role?: string;
    username?: string;
}