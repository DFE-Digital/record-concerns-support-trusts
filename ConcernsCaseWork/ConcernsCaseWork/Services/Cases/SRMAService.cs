using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public class SRMAService : ISRMAService
	{
		public Task<SRMAModel> GetSRMAById(long srmaId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn)
		{
			throw new NotImplementedException();
		}

		public Task SaveSRMA(SRMAModel srma)
		{
			throw new NotImplementedException();
		}

		public Task SetDateAccepted(long srmaId, DateTime? acceptedDate)
		{
			throw new NotImplementedException();
		}

		public Task SetDateClosed(long srmaId, DateTime? ClosedDate)
		{
			throw new NotImplementedException();
		}

		public Task SetDateReportSent(long srmaId, DateTime? reportSentDate)
		{
			throw new NotImplementedException();
		}

		public Task SetNotes(long srmaId, string notes)
		{
			throw new NotImplementedException();
		}

		public Task SetOfferedDate(long srmaId, DateTime offeredDate)
		{
			throw new NotImplementedException();
		}

		public Task SetReason(long srmaId, SRMAReasonOffered reason)
		{
			throw new NotImplementedException();
		}

		public Task SetStatus(long srmaId, SRMAStatus status)
		{
			throw new NotImplementedException();
		}

		public Task SetVisitDates(long srmaId, DateTime startDate, DateTime? endDate)
		{
			throw new NotImplementedException();
		}
	}
}
