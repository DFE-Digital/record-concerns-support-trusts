import { EnvApi, EnvApiKey, EnvUsername } from "../constants/cypressConstants";
import { ApiBase } from "./apiBase";
import { CreateCaseRequest, CreateCaseResponse, PatchCaseRequest, PatchCaseResponse, ResponseWrapper } from "./apiDomain";

class CaseApi extends ApiBase {
    public get(caseId: number): Cypress.Chainable<CreateCaseResponse> {
        return cy.request<ResponseWrapper<CreateCaseResponse>>({
            method: 'GET',
            url: Cypress.env(EnvApi) + `/v2/concerns-cases/urn/${caseId}`,
            headers: this.getHeaders()
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
            trustUkprn: this.chooseRandomTrust(),
            deEscalation: new Date().toISOString(),
            issue: "test",
            currentStatus: "current status",
            caseAim: "case aim",
            deEscalationPoint: "de-escalation point",
            nextSteps: "next steps",
            caseHistory: "case history",
            statusId: 1,
            ratingId: 4,
            territory: 1
        }

        return cy.request<ResponseWrapper<CreateCaseResponse>>({
            method: 'POST',
            url: Cypress.env(EnvApi) + "/v2/concerns-cases/",
            headers: this.getHeaders(),
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
            headers: this.getHeaders(),
            body: request
        })
        .then((response => {
            return response.body.data;
        }));
    }

    private chooseRandomTrust()
    {
        let searchTerm = [
            "10058372",
            "10060045",
            "10060278",
            "10058372",
            "10059732",
            "10060186",
            "10080822",
            "10081341",
            "10058833",
            "10058354",
            "10066108",
            "10058598",
            "10059919",
            "10057355",
            "10058295",
            "10059877",
            "10060927",
            "10059550",
            "10058417",
            "10059171",
            "10060716",
            "10060832",
            "10066116",
            "10058998",
            "10058772",
            "10059020",
            "10058154",
            "10059577",
            "10059981",
            "10058198",
            "10060069",
            "10059834",
            "10064323",
            "10060619",
            "10058893",
            "10058873",
            "10060447",
            "10057945",
            "10058340",
            "10058890",
            "10059880",
            "10060445",
            "10058715",
            "10059448",
            "10060131",
            "10058725",
            "10058630",
            "10060260",
            "10058560",
            "10058776",
            "10059501",
            "10058240",
            "10059063",
            "10059055",
            "10060233",
            "10058723",
            "10059998",
            "10058813",
            "10059324",
            "10058181",
            "10061208",
            "10060877",
            "10058468",
            "10064307",
        ];
    
        let trustIndex = Math.floor(Math.random() * searchTerm.length);
        let result = searchTerm[trustIndex];

        return result;
    }
}

const caseApi = new CaseApi();

export default caseApi;