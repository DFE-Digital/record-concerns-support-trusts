export type ResponseWrapper<T> =
{
    data: T;
    paging: PagingResponse
}

export type PagingResponse =
{
    recordCount: number
};

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
    trustCompaniesHouseNumber: string;
};

export type PatchCaseRequest =
{
    urn: number;
    createdBy: string;
};

export type PatchCaseResponse =
{

};

export type GetConcernResponse =
{
    meansOfReferralId: number;
};

export type GetOpenCasesByOwnerResponse =
{

};

export type GetOpenCasesByTrustResponse =
{

};


export type CreateCityTechnologyCollegeRequest =
{
    name: string
    ukprn: string,
    companiesHouseNumber: string,
    addressline1: string,
    addressline2: string,
    addressline3: string,
    town: string,
    county: string,
    postcode: string,
};

export type CreateCityTechnologyCollegeResponse =
{

};

export type GetCityTechnologyCollegeResponse =
{
    name: string
    ukprn: string,
    companiesHouseNumber: string,
    addressline1: string,
    addressline2: string,
    addressline3: string,
    town: string,
    county: string,
    postcode: string,
};