import { EnvApiKey, EnvUsername, CaseworkerClaim } from "cypress/constants/cypressConstants";

export class ApiBase
{
    protected getHeaders(): object
    {
        const result = {
            ApiKey: Cypress.env(EnvApiKey),
            "Content-type": "application/json",
            "x-user-context-role-0" : CaseworkerClaim,
            "x-user-context-name" : Cypress.env(EnvUsername)
        };
        return result;
    }
}