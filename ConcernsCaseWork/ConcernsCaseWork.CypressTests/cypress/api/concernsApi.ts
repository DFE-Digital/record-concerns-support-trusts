import { EnvApi } from "../constants/cypressConstants";
import { ApiBase } from "./apiBase";
import { GetConcernResponse, ResponseWrapper } from "./apiDomain";

class ConcernsApi extends ApiBase
{
    public get(caseId: number): Cypress.Chainable<Array<GetConcernResponse>>
    {
        return cy.request<ResponseWrapper<Array<GetConcernResponse>>>({
            method: 'GET',
            url: Cypress.env(EnvApi) + `/v2/concerns-records/case/urn/${caseId}`,
            headers: this.getHeaders(),
        })
        .then(response =>
        {
            return response.body.data;
        });
    }

    public post(caseId: number): Cypress.Chainable
    {
        return cy.request({
            method: 'POST',
            url: Cypress.env(EnvApi) + "/v2/concerns-records/",
            headers: this.getHeaders(),
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