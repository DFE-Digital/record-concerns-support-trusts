import { getUkLocalDateFormatted } from 'cypress/support/formatDate';
import { EnvApi } from '../constants/cypressConstants';
import { ApiBase } from './apiBase';
import { CreateConcernOptions, GetConcernResponse, ResponseWrapper } from './apiDomain';

class ConcernsApi extends ApiBase {
    public get(caseId: number): Cypress.Chainable<Array<GetConcernResponse>> {
        return cy
            .request<ResponseWrapper<Array<GetConcernResponse>>>({
                method: 'GET',
                url: Cypress.env(EnvApi) + `/v2/concerns-records/case/urn/${caseId}`,
                headers: this.getHeaders(),
            })
            .then((response) => {
                return response.body.data;
            });
    }

    public post(caseId: number, options?: CreateConcernOptions): Cypress.Chainable {
        const currentDate = getUkLocalDateFormatted();

        return cy.request({
            method: 'POST',
            url: Cypress.env(EnvApi) + '/v2/concerns-records/',
            headers: this.getHeaders(),
            body: {
                createdAt: currentDate,
                reviewAt: currentDate,
                name: options?.name ?? 'Governance and compliance',
                description: options?.description ?? 'Compliance',
                reason: options?.reason ?? 'Governance and compliance: Compliance',
                caseUrn: caseId,
                typeId: options?.typeId ?? 30,
                ratingId: options?.ratingId ?? 4,
                statusId: 1,
                meansOfReferralId: options?.meansOfReferralId ?? 1,
            },
        });
    }
}

const concernsApi = new ConcernsApi();

export default concernsApi;
