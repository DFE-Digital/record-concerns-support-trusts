import { EnvJwtToken, EnvUsername, CaseworkerClaim } from "cypress/constants/cypressConstants";

export class ApiBase
{
    protected getHeaders(): object
    {
        const result = {
            Authorization: Cypress.env(EnvJwtToken),
            "Content-type": "application/json",
            "x-user-context-role-0" : CaseworkerClaim,
            "x-user-context-name" : Cypress.env(EnvUsername),
            "x-cypress-user": "cypressUser"
        };
        return result;
    }
}