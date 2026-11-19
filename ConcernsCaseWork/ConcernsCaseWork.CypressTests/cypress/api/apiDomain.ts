export type ResponseWrapper<T> = {
    data: T;
    paging: PagingResponse;
};

export type PagingResponse = {
    recordCount: number;
};

export type CreateCaseRequest = {
    createdAt: string;
    reviewAt: string;
    createdBy: string;
    trustUkprn: string;
    deEscalation?: string;
    issue?: string;
    currentStatus?: string;
    caseAim?: string;
    deEscalationPoint?: string;
    nextSteps?: string;
    caseHistory?: string;
    statusId?: number;
    ratingId?: number;
    territory?: number;
    division?: number;
    region?: number;
};

export type CreateCaseResponse = {
    urn: number;
    createdBy: string;
    createdAt: string;
    trustCompaniesHouseNumber: string;
    trustUkprn: string;
    division?: number;
    territory?: number;
    region?: number;
    statusId?: number;
    ratingId?: number;
};

export type PatchCaseRequest = {
    urn: number;
    trustUkprn?: string;
    createdBy: string;
    division?: number;
    territory?: number;
    region?: number;
    statusId?: number;
    ratingId?: number;
    // Must be non-null: UpdateConcernsCase dereferences it before null-coalescing it.
    ratingRationalCommentary?: string;
};

export type PatchCaseResponse = {};

export type GetConcernResponse = {
    meansOfReferralId: number;
};

export type CreateConcernOptions = {
    typeId?: number;
    ratingId?: number;
    meansOfReferralId?: number;
    name?: string;
    description?: string;
    reason?: string;
};

export type GetOpenCasesByOwnerResponse = {};

export type GetOpenCasesForTeamByOwnerResponse = {};

export type GetOpenCasesByTrustResponse = {};

export type CreateCityTechnologyCollegeRequest = {
    name: string;
    ukprn: string;
    companiesHouseNumber: string;
    addressline1: string;
    addressline2: string;
    addressline3: string;
    town: string;
    county: string;
    postcode: string;
};

export type CreateCityTechnologyCollegeResponse = {};

export type GetCityTechnologyCollegeResponse = {
    name: string;
    ukprn: string;
    companiesHouseNumber: string;
    addressline1: string;
    addressline2: string;
    addressline3: string;
    town: string;
    county: string;
    postcode: string;
};

export type PutTeamRequest = {
    OwnerID: string;
    TeamMembers: Array<string>;
};

export type PutTeamResponse = {};

export type GetTeamByOwnerResponse = {
    ownerid: string;
    teamMembers: Array<string>;
};
