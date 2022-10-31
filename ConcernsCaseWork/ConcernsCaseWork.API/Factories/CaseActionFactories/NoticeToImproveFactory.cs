using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Factories.CaseActionFactories
{
    public static class NoticeToImproveFactory
    {
        public static NoticeToImprove CreateDBModel(CreateNoticeToImproveRequest createNoticeToImproveRequest)
        {
            return new NoticeToImprove
            {
                CaseUrn = createNoticeToImproveRequest.CaseUrn,
                DateStarted = createNoticeToImproveRequest.DateStarted,
                StatusId = createNoticeToImproveRequest.StatusId,
                Notes = createNoticeToImproveRequest.Notes,
                CreatedAt = createNoticeToImproveRequest.CreatedAt,
                CreatedBy = createNoticeToImproveRequest.CreatedBy,
                UpdatedAt = createNoticeToImproveRequest.UpdatedAt,
                ClosedAt = createNoticeToImproveRequest.ClosedAt,
                ClosedStatusId = createNoticeToImproveRequest.ClosedStatusId,
                NoticeToImproveReasonsMapping = createNoticeToImproveRequest.NoticeToImproveReasonsMapping.Select(r => {
                    return new NoticeToImproveReasonMapping()
                    {
                        NoticeToImproveReasonId = r
                    };
                }).ToList(),
                NoticeToImproveConditionsMapping = createNoticeToImproveRequest.NoticeToImproveConditionsMapping.Select(c => {
                    return new NoticeToImproveConditionMapping()
                    {
                        NoticeToImproveConditionId = c
                    };
                }).ToList()
            };
        }

        public static NoticeToImprove CreateDBModel(PatchNoticeToImproveRequest patchNoticeToImproveRequest)
        {
            return new NoticeToImprove
            {
                Id = patchNoticeToImproveRequest.Id,
                CaseUrn = patchNoticeToImproveRequest.CaseUrn,
                DateStarted = patchNoticeToImproveRequest.DateStarted,
                StatusId = patchNoticeToImproveRequest.StatusId,
                Notes = patchNoticeToImproveRequest.Notes,
                CreatedAt = patchNoticeToImproveRequest.CreatedAt,
                CreatedBy = patchNoticeToImproveRequest.CreatedBy,
                UpdatedAt = patchNoticeToImproveRequest.UpdatedAt,
                ClosedAt = patchNoticeToImproveRequest.ClosedAt,
                ClosedStatusId = patchNoticeToImproveRequest.ClosedStatusId,
                SumissionDecisionId = patchNoticeToImproveRequest.SumissionDecisionId,
                DateNTILifted = patchNoticeToImproveRequest.DateNTILifted,
                DateNTIClosed = patchNoticeToImproveRequest.DateNTIClosed,
                NoticeToImproveReasonsMapping = patchNoticeToImproveRequest.NoticeToImproveReasonsMapping.Select(r => {
                    return new NoticeToImproveReasonMapping()
                    {
                        NoticeToImproveReasonId = r
                    };
                }).ToList(),
                NoticeToImproveConditionsMapping = patchNoticeToImproveRequest.NoticeToImproveConditionsMapping.Select(c => {
                    return new NoticeToImproveConditionMapping()
                    {
                        NoticeToImproveConditionId = c
                    };
                }).ToList()
            };
        }

        public static NoticeToImproveResponse CreateResponse(NoticeToImprove model)
        {
            return new NoticeToImproveResponse
            {
                Id = model.Id,
                CaseUrn = model.CaseUrn,
                DateStarted = model.DateStarted,
                StatusId = model.StatusId,
                Notes = model.Notes,
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy,
                UpdatedAt = model.UpdatedAt,
                ClosedAt = model.ClosedAt,
                ClosedStatusId = model.ClosedStatusId,
                SumissionDecisionId = model.SumissionDecisionId,
                DateNTILifted = model.DateNTILifted,
                DateNTIClosed = model.DateNTIClosed,
                NoticeToImproveReasonsMapping = model.NoticeToImproveReasonsMapping.Select(r => { return r.NoticeToImproveReasonId; }).ToList(),
                NoticeToImproveConditionsMapping = model.NoticeToImproveConditionsMapping.Select(c => { return c.NoticeToImproveConditionId; }).ToList(),
            };
        }
    }
}
