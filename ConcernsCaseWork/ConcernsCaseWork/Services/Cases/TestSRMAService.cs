using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public class TestSRMAService : ISRMAService
	{
		private readonly List<SRMAModel> SRMAs;
		private readonly Random random;
		private readonly bool autoGenerateSRMAs = false;

		public TestSRMAService()
		{
			SRMAs = new List<SRMAModel>();
			random = new Random(DateTime.Now.Millisecond);
		}

		public Task SaveSRMA(SRMAModel srma)
		{
			srma.Id = random.Next(100000);
			SRMAs.Add(srma);

			return Task.CompletedTask;
		}

		public Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn)
		{
			var srmaList = SRMAs.Where(s => s.CaseUrn == caseUrn);

			if (autoGenerateSRMAs)
				return Task.FromResult(srmaList?.Count() > 0 ? srmaList : CreateTestData(caseUrn));
			else
				return Task.FromResult(srmaList);
		}

		private List<SRMAModel> CreateTestData(long caseUrn)
		{
			var now = DateTime.Now;

			var testSRMA = new SRMAModel(now.Millisecond, 
										caseUrn, 
										now, 
										now, 
										now, 
										now, 
										now, 
										SRMAStatus.TrustConsidering, 
										"Auto generated test data", 
										SRMAReasonOffered.RDDIntervention,
										now);


			SRMAs.Add(testSRMA);
			return SRMAs;
		}

		public Task<SRMAModel> GetSRMAById(long srmaId)
		{
			return Task.FromResult(SRMAs.SingleOrDefault(s => s.Id == srmaId));
		}

		public Task SetStatus(long srmaId, SRMAStatus status)
		{
			SRMAs.Single(s => s.Id == srmaId).Status = status;
			return Task.CompletedTask;
		}

		public Task SetReason(long srmaId, SRMAReasonOffered reason)
		{
			SRMAs.Single(s => s.Id == srmaId).Reason = reason;
			return Task.CompletedTask;
		}

		public Task SetOfferedDate(long srmaId, DateTime offeredDate)
		{
			SRMAs.Single(s => s.Id == srmaId).DateOffered = offeredDate;
			return Task.CompletedTask;
		}

		public Task SetNotes(long srmaId, string notes)
		{
			SRMAs.Single(s => s.Id == srmaId).Notes = notes;
			return Task.CompletedTask;
		}

		public Task SetVisitDates(long srmaId, DateTime startDate, DateTime? endDate)
		{
			var srma = SRMAs.Single(s => s.Id == srmaId);

			srma.DateVisitStart = startDate;
			srma.DateVisitEnd = endDate;
			return Task.CompletedTask;
		}

		public Task SetDateAccepted(long srmaId, DateTime? acceptedDate)
		{
			SRMAs.Single(s => s.Id == srmaId).DateAccepted = acceptedDate;
			return Task.CompletedTask;
		}

		public Task SetDateReportSent(long srmaId, DateTime? reportSentDate)
		{
			SRMAs.Single(s => s.Id == srmaId).DateReportSentToTrust = reportSentDate;
			return Task.CompletedTask;
		}

		public Task SetDateClosed(long srmaId, DateTime closedDate)
		{
			SRMAs.Single(s => s.Id == srmaId).ClosedAt = closedDate;
			return Task.CompletedTask;
		}
	}
}
