import { EnvApi, EnvApiKey } from "../constants/cypressConstants";

class ConcernsApi
{
    public post(caseId: number): Cypress.Chainable
    {
        return cy.request({
            method: 'POST',
            url: Cypress.env(EnvApi) + "/v2/concerns-records/",
            headers: {
                ApiKey: Cypress.env(EnvApiKey),
                "Content-type": "application/json"
            },
            body: {
                "createdAt": new Date().toISOString(),
                "reviewAt": new Date().toISOString(),
                "name": "Governance and compliance",
                "description": "Compliance",
                "reason": "Governance and compliance: Compliance",
                "caseUrn": caseId,
                "typeId": 23,
                "ratingId": 4,
                "statusId": 1,
                "meansOfReferralId": 1
            }
        });
    }
}

const concernsApi = new ConcernsApi();

export default concernsApi;