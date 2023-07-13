import { EnvApi, EnvUsername } from "../constants/cypressConstants";
import { ApiBase } from "./apiBase";
import { ResponseWrapper, CreateCityTechnologyCollegeRequest, CreateCityTechnologyCollegeResponse } from "./apiDomain";
import { CaseBuilder } from "./caseBuilder";

class CityTechnologyCollegeApi extends ApiBase {

    public post(request: CreateCityTechnologyCollegeRequest): Cypress.Chainable<CreateCityTechnologyCollegeResponse> {
        return cy.request<CreateCityTechnologyCollegeResponse>({
            method: 'POST',
            url: Cypress.env(EnvApi) + "/v2/citytechnologycolleges/",
            headers: this.getHeaders(),
            body: request
        })
        .then(response => {
            return response.body;
        });
    }
}

const cityTechnologyCollegeApi = new CityTechnologyCollegeApi();

export default cityTechnologyCollegeApi;