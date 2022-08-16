using Concerns.Data.Enums;
using Concerns.Data.Models;
using Concerns.Data.RequestModels.CaseActions.SRMA;
using Concerns.Data.ResponseModels.CaseActions.SRMA;

namespace Concerns.Data.Factories.CaseActionFactories
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
                ReasonId = (int?)(createSRMARequest.Reason == SRMAReasonOffered.Unknown ? null : createSRMARequest.Reason),
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
                Status = (Enums.SRMAStatus)model.StatusId,
                Reason = (Enums.SRMAReasonOffered?)model.ReasonId,
                Notes = model.Notes,
                Urn = model.Urn,
                CloseStatus = (Enums.SRMAStatus)(model.CloseStatusId ?? 0),
                UpdatedAt = model.UpdatedAt,
                ClosedAt = model.ClosedAt,
                CreatedBy = model.CreatedBy
            };
        }

 
    }
}
