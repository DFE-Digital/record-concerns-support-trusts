using Concerns.Data.Enums;
using Concerns.Data.Models;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;

namespace ConcernsCaseWork.API.Factories.CaseActionFactories
{
    public static class SRMAFactory
    {
        public static SRMACase CreateDBModel(CreateSRMARequest createSRMARequest)
        {
            return new SRMACase
            {
                Id = createSRMARequest.Id,
                CaseUrn = createSRMARequest.CaseUrn,
                CreatedAt = createSRMARequest.CreatedAt,
                DateOffered = createSRMARequest.DateOffered,
                DateReportSentToTrust = createSRMARequest.DateReportSentToTrust,
                StartDateOfVisit = createSRMARequest.DateVisitStart,
                EndDateOfVisit = createSRMARequest.DateVisitEnd,
                DateAccepted = createSRMARequest.DateAccepted,
                StatusId = (int)createSRMARequest.Status,
                ReasonId = (int?)(createSRMARequest.Reason == SRMAReasonOfferedEnum.Unknown ? null : createSRMARequest.Reason),
                Notes = createSRMARequest.Notes
            };
        }

        public static SRMAResponse CreateResponse(SRMACase model)
        {
            return new SRMAResponse
            {
                Id = model.Id,
                CaseUrn = model.CaseUrn,
                CreatedAt = model.CreatedAt,
                DateOffered = model.DateOffered,
                DateReportSentToTrust = model.DateReportSentToTrust,
                DateVisitStart = model.StartDateOfVisit,
                DateVisitEnd = model.EndDateOfVisit,
                DateAccepted = model.DateAccepted,
                Status = (SRMAStatusEnum)model.StatusId,
                Reason = (SRMAReasonOfferedEnum?)model.ReasonId,
                Notes = model.Notes,
                Urn = model.Urn,
                CloseStatus = (SRMAStatusEnum)(model.CloseStatusId ?? 0),
                UpdatedAt = model.UpdatedAt,
                ClosedAt = model.ClosedAt,
                CreatedBy = model.CreatedBy
            };
        }

 
    }
}
