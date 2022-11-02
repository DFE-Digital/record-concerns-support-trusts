using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.Decisions;
using FluentAssertions;
using NUnit.Framework;
using Service.TRAMS.Decision;
using System;
using System.Linq;

namespace ConcernsCaseWork.Tests.Services.Decisions
{
	[Parallelizable(ParallelScope.All)]
	internal class DecisionMappingTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public void MapDtoToModel_ReturnsCorrectModel()
		{
			var apiDecision = _fixture.Create<GetDecisionResponseDto>();
			apiDecision.Status = DecisionStatus.InProgress;
			apiDecision.CreatedAt = new DateTimeOffset(2023, 1, 4, 0, 0, 0, new TimeSpan());
			apiDecision.ClosedAt = new DateTimeOffset(2023, 2, 24, 0, 0, 0, new TimeSpan());

			var result = DecisionMapping.MapDtoToModel(apiDecision);

			AssertDecisionModel(result, apiDecision);

			result.StatusName.Should().Be("In progress");
			result.OpenedDate.Should().Be("04-01-2023");
			result.ClosedDate.Should().Be("24-02-2023");
		}

		[Test]
		public void MapDtoModelList_ReturnsCorrectModel()
		{
			var apiDecisions = _fixture.CreateMany<GetDecisionResponseDto>().ToList();

			var result = DecisionMapping.MapDtoToModel(apiDecisions);

			result.Should().HaveCount(apiDecisions.Count);

			for (var idx = 0; idx < apiDecisions.Count; idx++)
			{
				var decision = apiDecisions[idx];
				var model = result[idx];

				AssertDecisionModel(model, decision);
			}
		}

		private void AssertDecisionModel(ActionSummary model, GetDecisionResponseDto decision)
		{
			model.CaseUrn.Should().Be(decision.ConcernsCaseUrn);
			model.Name.Should().Be($"Decision: {decision.Title}");
			model.RelativeUrl.Should().Be($"/case/{decision.ConcernsCaseUrn}/management/action/decision/{decision.DecisionId}");
		}
	}
}
