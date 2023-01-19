export type ResponseWrapper<T> =
{
    data: T;
}

export type CreateCaseRequest =
{
    createdAt: string
    reviewAt: string,
    createdBy: string,
    trustUkprn: string,
    deEscalation: string,
    issue: string,
    currentStatus: string,
    caseAim: string,
    deEscalationPoint: string,
    nextSteps: string,
    caseHistory: string,
    statusId: number,
    ratingId: number,
    territory: 1
};

export type CreateCaseResponse =
{
    urn: number;
    createdBy: string;
};

export type PatchCaseRequest =
{
    urn: number;
    createdBy: string;
}

export type PatchCaseResponse =
{

}