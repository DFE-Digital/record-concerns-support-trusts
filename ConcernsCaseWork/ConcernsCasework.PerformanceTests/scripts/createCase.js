import http from 'k6/http';
import { check, sleep } from 'k6';
import getConfig from '../getConfig.js';
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js'

export const options = {
    vus: 400,
    duration: '30s',
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
    const createdCase = createCase();

    createConcern(createdCase.urn);

    const srma = createSrma(createdCase.urn, createdCase.createdBy);
    // editSrma(srma.id);

    createFinancialPlan(createdCase.urn, createdCase.createdBy);

    createNti(createdCase.urn, createdCase.createdBy);

    createNtiWarningLetter(createdCase.urn, createdCase.createdBy);

    createNtiUnderConsideration(createdCase.urn, createdCase.createdBy);

    const decision = createDecision(createdCase.urn);
    createDecisionOutcome(createdCase.urn, decision.decisionId);

    sleep(1);
}

function createCase() {
    const request =
    {
        createdAt: new Date().toISOString(),
        reviewAt: new Date().toISOString(),
        createdBy: uuidv4(),
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

    const response = http.post(
        `${config.url}/v2/concerns-cases`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created case successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function createConcern(caseId) {
    const request = {
        createdAt: new Date().toISOString(),
        reviewAt: new Date().toISOString(),
        name: "Governance and compliance",
        description: "Compliance",
        reason: "Governance and compliance: Compliance",
        caseUrn: caseId,
        typeId: 23,
        ratingId: 4,
        statusId: 1,
        meansOfReferralId: 1
    };

    const response = http.post(
        `${config.url}/v2/concerns-records/`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created concern successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function createSrma(caseId, createdBy) {
    const request = {
        id: 0,
        caseUrn: caseId,
        "createdAt": new Date().toISOString(),
        "createdBy": createdBy,
        "dateOffered": new Date().toISOString(),
        "dateAccepted": new Date().toISOString(),
        "dateReportSentToTrust": new Date().toISOString(),
        "dateVisitStart": new Date().toISOString(),
        "dateVisitEnd": new Date().toISOString(),
        "status": 1,
        "notes": "This is my notes",
        "reason": 1
    };

    const response = http.post(
        `${config.url}/v2/case-actions/srma`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created SRMA successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function editSrma(srmaId) {
    getSrma(srmaId);

    const response = http.patch(
        `${config.url}/v2/case-actions/srma/${srmaId}/update-notes?notes=These are the new notes`,
        {},
        {
            headers: getHeaders()
        });

    check(response, {
        'Edited SRMA successfully': (res) => res.status === 200
    });
}

function getSrma(srmaId) {
    const response = http.get(
        `${config.url}/v2/case-actions/srma?srmaId=${srmaId}`,
        {
            headers: getHeaders()
        });

    check(response, {
        'GET SRMA successfully': (res) => res.status === 200
    });
}

function createFinancialPlan(caseId, createdBy) {
    const request = {
        caseUrn: caseId,
        name: "financial",
        statusId: 1,
        datePlanRequested: new Date().toISOString(),
        dateViablePlanReceived: new Date().toISOString(),
        createdAt: new Date().toISOString(),
        createdBy: createdBy,
        updatedAt: new Date().toISOString(),
        closedAt: new Date().toISOString(),
        notes: "This is my notes"
    };

    const response = http.post(
        `${config.url}/v2/case-actions/financial-plan`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created Financial Plan successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function createNti(caseId, createdBy) {
    const request = {
        "caseUrn": caseId,
        "statusId": 1,
        "dateStarted": new Date().toISOString(),
        "notes": "This is my notes",
        "createdBy": createdBy,
        "createdAt": new Date().toISOString(),
        "updatedAt": new Date().toISOString(),
        "closedAt": new Date().toISOString(),
        // "closedStatusId": 0,
        "noticeToImproveReasonsMapping": [],
        "noticeToImproveConditionsMapping": []
    };

    const response = http.post(
        `${config.url}/v2/case-actions/notice-to-improve`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created NTI successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function createNtiWarningLetter(caseId, createdBy) {
    const request = {
        "caseUrn": caseId,
        "dateLetterSent": new Date().toISOString(),
        "notes": "This is my notes",
        "statusId": 1,
        "warningLetterReasonsMapping": [],
        "warningLetterConditionsMapping": [],
        "createdBy": createdBy,
        "createdAt": new Date().toISOString(),
        "updatedAt": new Date().toISOString(),
        "closedAt": new Date().toISOString(),
        // "closedStatusId": 0
    };

    const response = http.post(
        `${config.url}/v2/case-actions/nti-warning-letter`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created NTI warning letter successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function createNtiUnderConsideration(caseId, createdBy) {
    const request = {
        "caseUrn": caseId,
        "notes": "This is my notes",
        "underConsiderationReasonsMapping": [],
        "createdBy": createdBy,
        "createdAt": new Date().toISOString(),
        "updatedAt": new Date().toISOString(),
        "closedAt": new Date().toISOString(),
        // "closedStatusId": 0
    };

    const response = http.post(
        `${config.url}/v2/case-actions/nti-under-consideration`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created NTI under consideration successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function createDecision(caseId) {
    const request = {
        "concernsCaseUrn": caseId,
        "decisionTypes": [],
        "totalAmountRequested": 100,
        "supportingNotes": "This is my notes",
        "receivedRequestDate": new Date().toISOString(),
        "submissionDocumentLink": "link",
        "submissionRequired": true,
        "retrospectiveApproval": true,
        "crmCaseNumber": "123456"
    };

    const response = http.post(
        `${config.url}/v2/concerns-cases/${caseId}/decisions`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created decision successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}

function createDecisionOutcome(caseId, decisionId) {
    const request = {
        "decisionOutcomeId": 0,
        "status": 1,
        "totalAmount": 100,
        "decisionMadeDate": new Date().toISOString(),
        "decisionEffectiveFromDate": new Date().toISOString(),
        "authorizer": 1,
        "businessAreasConsulted": []
    };

    const response = http.post(
        `${config.url}/v2/concerns-cases/${caseId}/decisions/${decisionId}/outcome`,
        JSON.stringify(request),
        {
            headers: getHeaders()
        });

    check(response, {
        'Created decision outcome successfully': (res) => res.status === 201
    });

    const result = JSON.parse(response.body).data;

    return result;
}