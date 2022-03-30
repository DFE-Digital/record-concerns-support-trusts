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

		public TestSRMAService()
		{
			SRMAs = new List<SRMAModel>();
			random = new Random(DateTime.Now.Millisecond);
		}

		public Task SaveSRMA(SRMAModel srma)
		{
			srma.Notes = "TEST DATA: " + srma.Notes;
			srma.Id = random.Next(100000);
			SRMAs.Add(srma);

			return Task.CompletedTask;
		}

		public Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn)
		{
			var srmaList = SRMAs.Where(s => s.CaseUrn == caseUrn);
			// return Task.FromResult(srmaList?.Count() > 0 ? srmaList : CreateTestData(caseUrn));
			return Task.FromResult(srmaList);
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
