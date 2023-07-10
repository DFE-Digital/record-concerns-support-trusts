import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js'
import getConfig from '../getConfig.js';

// const config = getConfig();

export function buildCase()
{
    const result =
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

    return result;
}

export function buildConcern(caseId)
{
    const result = {
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

    return result;
}

export function buildClosedCase()
{
    const result = buildCase();

    result.closedAt = new Date().toISOString();
    result.statusId = 3;

    return result;
}

export function buildClosedConcern(caseId)
{
    const result = buildConcern(caseId);
    result.closedAt = new Date().toISOString();
    result.statusId = 3;

    return result;
}