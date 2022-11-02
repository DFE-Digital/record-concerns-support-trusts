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
		public void ToActionSummary_ReturnsCorrectModel()
		{
			var apiDecision = _fixture.Create<GetDecisionResponseDto>();
			apiDecision.Status = DecisionStatus.InProgress;
			apiDecision.CreatedAt = new DateTimeOffset(2023, 1, 4, 0, 0, 0, new TimeSpan());
			apiDecision.ClosedAt = new DateTimeOffset(2023, 2, 24, 0, 0, 0, new TimeSpan());

			var result = DecisionMapping.ToActionSummary(apiDecision);

			result.StatusName.Should().Be("In progress");
			result.OpenedDate.Should().Be("04-01-2023");
			result.ClosedDate.Should().Be("24-02-2023");
			result.Name.Should().Be($"Decision: {apiDecision.Title}");
			result.RelativeUrl.Should().Be($"/case/{apiDecision.ConcernsCaseUrn}/management/action/decision/{apiDecision.DecisionId}");
		}
	}
}
