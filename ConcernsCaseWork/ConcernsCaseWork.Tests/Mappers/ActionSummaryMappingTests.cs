using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class ActionSummaryMappingTests
	{
		[Test]
		public void Map_Returns_CorrectModel()
		{
			var openDecision = new ActionSummaryModel() { RawOpenedDate = new DateTimeOffset(new DateTime(2020, 12, 12)) };
			var openSrma = new ActionSummaryModel() { RawOpenedDate = new DateTimeOffset(new DateTime(2020, 7, 7)) };
			var openNti = new ActionSummaryModel() { RawOpenedDate = new DateTimeOffset(new DateTime(2020, 1, 1)) };
			var openFinancialPlan = new ActionSummaryModel() { RawOpenedDate = new DateTimeOffset(new DateTime(2020, 1, 3)) };

			var closedDecision = new ActionSummaryModel() { RawClosedDate = new DateTimeOffset(new DateTime(2020, 12, 12)) };
			var closedSrma = new ActionSummaryModel() { RawClosedDate = new DateTimeOffset(new DateTime(2020, 7, 7)) };
			var closedNti = new ActionSummaryModel() { RawClosedDate = new DateTimeOffset(new DateTime(2020, 1, 1)) };
			var closedFinancialPlan = new ActionSummaryModel() { RawClosedDate = new DateTimeOffset(new DateTime(2020, 1, 3)) };

			var input = new List<ActionSummaryModel>()
			{
				openDecision,
				openSrma,
				openNti,
				openFinancialPlan,
				closedDecision,
				closedSrma,
				closedNti,
				closedFinancialPlan
			};

			var result = ActionSummaryMapping.ToActionSummaryBreakdown(input);

			var expectedOpenCases = new List<ActionSummaryModel>() { openNti, openFinancialPlan, openSrma, openDecision };
			var expectedClosedCases = new List<ActionSummaryModel>() { closedNti, closedFinancialPlan, closedSrma, closedDecision };

			result.OpenActions.Should().BeEquivalentTo(expectedOpenCases, config => config.WithStrictOrdering());
			result.ClosedActions.Should().BeEquivalentTo(expectedClosedCases, config => config.WithStrictOrdering());
		}
	}
}
