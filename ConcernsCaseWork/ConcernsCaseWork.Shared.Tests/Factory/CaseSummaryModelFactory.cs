using AutoFixture;
using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseSummaryModelFactory
	{
		private readonly static Fixture Fixture = new Fixture();
		
		public static List<ActiveCaseSummaryModel> BuildActiveCaseSummaryModels()
			=> Fixture.CreateMany<ActiveCaseSummaryModel>().ToList();
		
		public static List<ClosedCaseSummaryModel> BuildClosedCaseSummaryModels()
			=> Fixture.CreateMany<ClosedCaseSummaryModel>().ToList();
	}
}