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

		public TestSRMAService()
		{
			SRMAs = new List<SRMAModel>();
		}

		public Task SaveSRMA(SRMAModel srma)
		{
			srma.Notes = "TEST DATA: " + srma.Notes;
			SRMAs.Add(srma);

			return Task.CompletedTask;
		}

		public Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn)
		{
			var srmaList = SRMAs.Where(s => s.CaseUrn == caseUrn);
			return Task.FromResult(srmaList?.Count() > 0 ? srmaList : CreateTestData(caseUrn));
		}

		public Task<SRMAModel> GetSRMAById(long srmaId)
		{
			return Task.FromResult(SRMAs.SingleOrDefault(s => s.Id == srmaId));
		}

		private List<SRMAModel> CreateTestData(long caseUrn)
		{
			var testSRMA = new SRMAModel
			{
				Id = DateTime.Now.Millisecond,
				CaseUrn = caseUrn,
				DateOffered = DateTime.Now,
				Notes = "Auto generated test data",
				Status = SRMAStatus.TrustConsidering,
			};

			SRMAs.Add(testSRMA);
			return SRMAs;
		}
	}
}
