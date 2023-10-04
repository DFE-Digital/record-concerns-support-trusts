using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.Data.Models;

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
                ReasonId = (int?)(createSRMARequest.Reason == Contracts.Srma.SRMAReasonOffered.Unknown ? null : createSRMARequest.Reason),
                Notes = createSRMARequest.Notes,
                CreatedBy = createSRMARequest.CreatedBy
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
                Status = (Contracts.Srma.SRMAStatus)model.StatusId,
                Reason = (Contracts.Srma.SRMAReasonOffered?)model.ReasonId,
                Notes = model.Notes,
                CloseStatus = (Contracts.Srma.SRMAStatus)(model.CloseStatusId ?? 0),
                UpdatedAt = model.UpdatedAt,
                ClosedAt = model.ClosedAt,
                CreatedBy = model.CreatedBy
            };
        }
 
    }
}
