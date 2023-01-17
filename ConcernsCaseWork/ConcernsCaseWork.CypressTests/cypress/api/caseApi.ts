import { EnvApi, EnvApiKey, EnvUsername } from "../constants/cypressConstants";
import { CreateCaseRequest, CreateCaseResponse, PatchCaseRequest, PatchCaseResponse, ResponseWrapper } from "./apiDomain";

class CaseApi {
    public get(caseId: number): Cypress.Chainable<CreateCaseResponse> {
        return cy.request<ResponseWrapper<CreateCaseResponse>>({
            method: 'GET',
            url: Cypress.env(EnvApi) + `/v2/concerns-cases/urn/${caseId}`,
            headers: {
                ApiKey: Cypress.env(EnvApiKey),
                "Content-type": "application/json"
            }
        })
            .then((response) => {
                return response.body.data;
            });
    }

    public post(): Cypress.Chainable<CreateCaseResponse> {

        const request: CreateCaseRequest =
        {
            createdAt: new Date().toISOString(),
            reviewAt: new Date().toISOString(),
            createdBy: Cypress.env(EnvUsername),
            trustUkprn: "10064172",
            deEscalation: new Date().toISOString(),
            issue: "test",
            currentStatus: "current status",
            caseAim: "case aim",
            deEscalationPoint: "de-escalation point",
            nextSteps: "next steps",
            caseHistory: "case history",
            urn: 1040438,
            statusId: 1,
            ratingId: 4,
            territory: 1
        }

        return cy.request<ResponseWrapper<CreateCaseResponse>>({
            method: 'POST',
            url: Cypress.env(EnvApi) + "/v2/concerns-cases/",
            headers: {
                ApiKey: Cypress.env(EnvApiKey),
                "Content-type": "application/json"
            },
            body: request
        })
        .then(response => {
            return response.body.data;
        });
    }

    public patch(caseId: number, request: PatchCaseRequest): Cypress.Chainable<PatchCaseResponse> {
        return cy.request<ResponseWrapper<PatchCaseResponse>>({
            method: 'PATCH',
            url: Cypress.env(EnvApi) + `/v2/concerns-cases/${caseId}`,
            headers: {
                ApiKey: Cypress.env(EnvApiKey),
                "Content-type": "application/json"
            },
            body: request
        })
        .then((response => {
            return response.body.data;
        }));
    }
}

const caseApi = new CaseApi();

export default caseApi;