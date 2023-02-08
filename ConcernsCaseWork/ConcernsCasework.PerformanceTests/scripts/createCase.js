import http from 'k6/http';
import { check, sleep } from 'k6';
import getConfig from '../getConfig.js';

export const options = {
    vus: 1,
    duration: '1s',
    // httpDebug: 'full',
};

const config = getConfig();

function getHeaders() {
    return {
        "ApiKey": "app-key",
        "Content-Type": "application/json",
        "x-user-context-role-0": "concerns-casework.caseworker",
        "x-user-context-name": config.username
    };
}

export default function () {

    const request =
    {
        createdAt: new Date().toISOString(),
        reviewAt: new Date().toISOString(),
        createdBy: config.username,
        trustUkprn: "10058598",
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
    };

    const createCaseResponse = http.post(
        `${config.url}/v2/concerns-cases`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(createCaseResponse.status, 200);

    const createdCase = JSON.parse(createCaseResponse.body).data;

    const permissionsRequest =
    {
        caseIds: [createdCase.urn]
    };

    const permissionsResponse = http.post(
        `${config.url}/v2/permissions`,
        JSON.stringify(permissionsRequest),
        {
            headers: getHeaders()
        }
    );

    check(permissionsResponse.status, 200);

    sleep(1);
}
