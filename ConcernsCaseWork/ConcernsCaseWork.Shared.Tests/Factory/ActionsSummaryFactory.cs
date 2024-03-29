using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Shared.Tests.Factory;

public static class ActionsSummaryFactory
{
	private readonly static Fixture _fixture = new();
	
	public static IList<ActionSummaryModel> BuildListOfActionSummaries()
		=> _fixture.CreateMany<ActionSummaryModel>().ToList();
}