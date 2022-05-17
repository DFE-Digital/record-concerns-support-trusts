using ConcernsCaseWork.Models.CaseActions;
using Service.TRAMS.CaseActions;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseActionsMapping
	{
		public static SRMAModel Map(SRMADto srmaDto)
		{
			return new SRMAModel
			{
				Id = srmaDto.Id,
				CaseUrn = srmaDto.CaseId,
				ClosedAt = srmaDto.ClosedAt,
				CreatedAt = srmaDto.DateOffered,
				DateOffered = srmaDto.DateOffered,
				DateAccepted = srmaDto.DateAccepted,
				DateReportSentToTrust = srmaDto.DateReportSentToTrust,
				DateVisitEnd = srmaDto.DateVisitEnd,
				DateVisitStart = srmaDto.DateVisitStart,
				Notes = srmaDto.Notes,
				Reason = (Enums.SRMAReasonOffered)srmaDto.Reason,
				Status = (Enums.SRMAStatus)srmaDto.Status
			};
		}
	}
}
