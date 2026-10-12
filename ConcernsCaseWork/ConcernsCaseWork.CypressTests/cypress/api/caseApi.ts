import { EnvApi, EnvUsername } from '../constants/cypressConstants';
import { ApiBase } from './apiBase';
import {
    CreateCaseRequest,
    CreateCaseResponse,
    GetOpenCasesByOwnerResponse,
    GetOpenCasesForTeamByOwnerResponse,
    GetOpenCasesByTrustResponse,
    PatchCaseRequest,
    PatchCaseResponse,
    ResponseWrapper,
    PutTeamRequest,
    PutTeamResponse,
    GetTeamByOwnerResponse,
} from './apiDomain';
import { CaseBuilder } from './caseBuilder';

class CaseApi extends ApiBase {
    public get(caseId: number): Cypress.Chainable<CreateCaseResponse> {
        return cy
            .request<ResponseWrapper<CreateCaseResponse>>({
                method: 'GET',
                url: Cypress.env(EnvApi) + `/v2/concerns-cases/urn/${caseId}`,
                headers: this.getHeaders(),
            })
            .then((response) => {
                return response.body.data;
            });
    }

    public post(request: CreateCaseRequest): Cypress.Chainable<CreateCaseResponse> {
        return cy
            .request<ResponseWrapper<CreateCaseResponse>>({
                method: 'POST',
                url: Cypress.env(EnvApi) + '/v2/concerns-cases/',
                headers: this.getHeaders(),
                body: request,
            })
            .then((response) => {
                return response.body.data;
            });
    }

    public patch(caseId: number, request: PatchCaseRequest): Cypress.Chainable<PatchCaseResponse> {
        const url = Cypress.env(EnvApi) + `/v2/concerns-cases/${caseId}`;
        const headers = this.getHeaders();

        return cy
            .request<ResponseWrapper<PatchCaseResponse>>({
                method: 'PATCH',
                url: url,
                headers: headers,
                body: request,
                // Without this, Cypress throws on any non-2xx before .then() runs,
                // so we never get a chance to inspect the response.
                failOnStatusCode: false,
            })
            .then((response) => {
                if (response.status < 200 || response.status >= 300) {
                    const body = typeof response.body === 'string' ? response.body : JSON.stringify(response.body);

                    // Cypress.log/console.log render immediately; cy.log would be queued
                    // and discarded by the throw below.
                    Cypress.log({
                        name: 'PATCH failed',
                        message: `${response.status} ${response.statusText} ${url}`,
                        consoleProps: () => ({
                            Status: response.status,
                            URL: url,
                            'Request headers': headers,
                            'Request body': request,
                            'Response headers': response.headers,
                            'Response body': response.body,
                        }),
                    });
                    console.log('PATCH %s failed with %d', url, response.status, {
                        requestHeaders: headers,
                        requestBody: request,
                        responseHeaders: response.headers,
                        responseBody: response.body,
                    });

                    throw new Error(`PATCH ${url} failed with status ${response.status}: ${body}`);
                }

                return response.body.data;
            });
    }

    public getOpenCasesByOwner(ownerId: string): Cypress.Chainable<ResponseWrapper<GetOpenCasesByOwnerResponse>> {
        return cy
            .request<ResponseWrapper<GetOpenCasesByOwnerResponse>>({
                method: 'GET',
                url: Cypress.env(EnvApi) + `/v2/concerns-cases/summary/${ownerId}/active?page=1&count=5`,
                headers: this.getHeaders(),
            })
            .then((response) => {
                return response.body;
            });
    }

    public getOpenCasesForTeamByOwner(
        ownerId: string
    ): Cypress.Chainable<ResponseWrapper<GetOpenCasesForTeamByOwnerResponse>> {
        return cy
            .request<ResponseWrapper<GetOpenCasesForTeamByOwnerResponse>>({
                method: 'GET',
                url: Cypress.env(EnvApi) + `/v2/concerns-cases/summary/${ownerId}/active/team?page=1&count=5`,
                headers: this.getHeaders(),
            })
            .then((response) => {
                return response.body;
            });
    }

    public getOpenCasesByTrust(trustUkPrn: string): Cypress.Chainable<ResponseWrapper<GetOpenCasesByTrustResponse>> {
        return cy
            .request<ResponseWrapper<GetOpenCasesByTrustResponse>>({
                method: 'GET',
                url: Cypress.env(EnvApi) + `/v2/concerns-cases/summary/bytrust/${trustUkPrn}/active?page=1&count=5`,
                headers: this.getHeaders(),
            })
            .then((response) => {
                return response.body;
            });
    }

    public put(ownerId: string, request: PutTeamRequest): Cypress.Chainable<PutTeamResponse> {
        return cy
            .request<ResponseWrapper<PutTeamResponse>>({
                method: 'PUT',
                url: Cypress.env(EnvApi) + `/v2/concerns-team-casework/owners/${ownerId}`,
                headers: this.getHeaders(),
                body: request,
            })
            .then((response) => {
                return response.body.data;
            });
    }

    public getTeamByTeam(ownerId: string): Cypress.Chainable<ResponseWrapper<GetTeamByOwnerResponse>> {
        return cy
            .request<ResponseWrapper<GetTeamByOwnerResponse>>({
                method: 'GET',
                url: Cypress.env(EnvApi) + `/v2/concerns-team-casework/owners/${ownerId}`,
                headers: this.getHeaders(),
            })
            .then((response) => {
                return response.body;
            });
    }
}

const caseApi = new CaseApi();

export default caseApi;
