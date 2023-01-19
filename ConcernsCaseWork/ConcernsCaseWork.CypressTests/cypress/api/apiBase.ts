import { EnvApiKey } from "cypress/constants/cypressConstants";

export class ApiBase
{
    protected getHeaders(): object
    {
        const result = {
            ApiKey: Cypress.env(EnvApiKey),
            "Content-type": "application/json"
        };

        return result;
    }
}