import { EnvApiKey, EnvUsername, CaseworkerClaim, jwtToken } from "cypress/constants/cypressConstants";

export class ApiBase
{
    protected getHeaders(): object
    {
        const result = {
            'Authorization': `Bearer ${Cypress.env(jwtToken)}`,
            "Content-type": "application/json",
            "x-user-context-role-0" : CaseworkerClaim,
            "x-user-context-name" : Cypress.env(EnvUsername)
        };
        return result;
    }
}