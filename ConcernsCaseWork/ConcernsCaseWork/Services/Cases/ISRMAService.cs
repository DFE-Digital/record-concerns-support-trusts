using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ISRMAService
	{
		public Task SaveSRMA(SRMAModel srma);
		public Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn);
		public Task<SRMAModel> GetSRMAById(long srmaId);
		public Task SetStatus(long srmaId, SRMAStatus status);
		public Task SetReason(long srmaId, SRMAReasonOffered reason);
		public Task SetOfferedDate(long srmaId, DateTime offeredDate);
		public Task SetNotes(long srmaId, string notes);
		public Task SetVisitDates(long srmaId, DateTime startDate, DateTime? endDate);
		public Task SetDateAccepted(long srmaId, DateTime? acceptedDate);
	}
}