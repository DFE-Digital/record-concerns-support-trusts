using AutoFixture;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Decisions;
using FluentAssertions;
using NUnit.Framework;
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

			var result = DecisionMapping.MapDtoToModel(apiDecision);

			AssertDecisionModel(result, apiDecision);
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

		private void AssertDecisionModel(DecisionModel model, GetDecisionResponseDto decision)
		{
			model.CaseUrn.Should().Be(decision.ConcernsCaseUrn);
			model.CreatedAt.Should().Be(decision.CreatedAt.Date);
			model.ClosedAt.Should().Be(decision.ClosedAt?.Date);
			model.Title.Should().Be(decision.Title);
		}
	}
}
