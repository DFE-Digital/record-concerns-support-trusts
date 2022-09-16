using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories.CaseActionFactories
{
    public static class NTIWarningLetterFactory
    {
        public static NTIWarningLetter CreateDBModel(CreateNTIWarningLetterRequest createNTIWarningLetterRequest)
        {
            return new NTIWarningLetter
            {
                CaseUrn = createNTIWarningLetterRequest.CaseUrn,
                WarningLetterReasonsMapping = createNTIWarningLetterRequest.WarningLetterReasonsMapping.Select(r => {
                    return new NTIWarningLetterReasonMapping()
                    {
                        NTIWarningLetterReasonId = r
                    };
                }).ToList(),
                WarningLetterConditionsMapping = createNTIWarningLetterRequest.WarningLetterConditionsMapping.Select(c => {
                    return new NTIWarningLetterConditionMapping()
                    {
                        NTIWarningLetterConditionId = c
                    };
                }).ToList(),
                DateLetterSent = createNTIWarningLetterRequest.DateLetterSent,
                StatusId = createNTIWarningLetterRequest.StatusId,
                Notes = createNTIWarningLetterRequest.Notes,
                CreatedAt = createNTIWarningLetterRequest.CreatedAt,
                CreatedBy = createNTIWarningLetterRequest.CreatedBy,
                UpdatedAt = createNTIWarningLetterRequest.UpdatedAt,
                ClosedAt = createNTIWarningLetterRequest.ClosedAt,
                ClosedStatusId = createNTIWarningLetterRequest.ClosedStatusId
            };
        }

        public static NTIWarningLetter CreateDBModel(PatchNTIWarningLetterRequest patchNTIWarningLetterRequest)
        {
            return new NTIWarningLetter
            {
                Id = patchNTIWarningLetterRequest.Id,
                CaseUrn = patchNTIWarningLetterRequest.CaseUrn,
                WarningLetterReasonsMapping = patchNTIWarningLetterRequest.WarningLetterReasonsMapping.Select(r => {
                    return new NTIWarningLetterReasonMapping()
                    {
                        NTIWarningLetterReasonId = r
                    };
                }).ToList(),
                WarningLetterConditionsMapping = patchNTIWarningLetterRequest.WarningLetterConditionsMapping.Select(c => {
                    return new NTIWarningLetterConditionMapping()
                    {
                        NTIWarningLetterConditionId = c
                    };
                }).ToList(),
                DateLetterSent = patchNTIWarningLetterRequest.DateLetterSent,
                StatusId = patchNTIWarningLetterRequest.StatusId,
                Notes = patchNTIWarningLetterRequest.Notes,
                CreatedAt = patchNTIWarningLetterRequest.CreatedAt,
                CreatedBy = patchNTIWarningLetterRequest.CreatedBy,
                UpdatedAt = patchNTIWarningLetterRequest.UpdatedAt,
                ClosedAt = patchNTIWarningLetterRequest.ClosedAt,
                ClosedStatusId = patchNTIWarningLetterRequest.ClosedStatusId
            };
        }

        public static NTIWarningLetterResponse CreateResponse(NTIWarningLetter model)
        {
            return new NTIWarningLetterResponse
            {
                Id = model.Id,
                CaseUrn = model.CaseUrn,
                DateLetterSent = model.DateLetterSent,
                Notes = model.Notes,
                StatusId = model.StatusId,
                WarningLetterReasonsMapping = model.WarningLetterReasonsMapping.Select(r => { return r.NTIWarningLetterReasonId; }).ToArray(),
                WarningLetterConditionsMapping = model.WarningLetterConditionsMapping.Select(r => { return r.NTIWarningLetterConditionId; }).ToArray(),
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy,
                UpdatedAt = model.UpdatedAt,
                ClosedAt = model.ClosedAt,
                ClosedStatusId = model.ClosedStatusId
            };
        }
    }
}
