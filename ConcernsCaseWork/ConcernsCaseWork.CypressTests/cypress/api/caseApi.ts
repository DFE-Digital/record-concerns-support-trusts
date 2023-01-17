import { EnvApi, EnvApiKey, EnvUsername } from "../constants/cypressConstants";

class CaseApi
{
    public post(): Cypress.Chainable
    {
        return cy.request({
            method: 'POST',
            url: Cypress.env(EnvApi) + "/v2/concerns-cases/",
            headers: {
                ApiKey: Cypress.env(EnvApiKey),
                "Content-type": "application/json"
            },
            body: {
                "createdAt": new Date().toISOString(),
                "reviewAt": new Date().toISOString(),
                "createdBy": Cypress.env(EnvUsername),
                "trustUkprn": "10064172",
                "deEscalation": new Date().toISOString(),
                "issue": "test",
                "currentStatus": "current status",
                "caseAim": "case aim",
                "deEscalationPoint": "de-escalation point",
                "nextSteps": "next steps",
                "caseHistory": "case history",
                "urn": 1040438,
                "statusId": 1,
                "ratingId": 4,
                "territory": 1
            }
        })
        .then(response =>
        {
            return response.body;
        });
    }
}

const caseApi = new CaseApi();

export default caseApi;