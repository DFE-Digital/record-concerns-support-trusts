using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.Decision.Outcome;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision.Outcome
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class AddPageModelTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public async Task OnGetAsync_When_NewDecisionOutcome_Returns_Page()
		{
			const int expectedUrn = 2;
			const int expectedDecisionId = 7;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithDecisionId(expectedDecisionId)
				.BuildSut();

			await sut.OnGetAsync();

			sut.ViewData[ViewDataConstants.Title].Should().Be("Add outcome");

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.CaseUrn.Should().Be(expectedUrn);
			sut.DecisionId.Should().Be(expectedDecisionId);
			sut.BusinessAreaCheckBoxes.Should().NotBeEmpty();
			sut.SaveAndContinueButtonText.Should().Be("Save and return to case overview");
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecision_Returns_Page()
		{
			const int expectedCaseUrn = 1;
			const int expectedDecisionId = 2;
			const int expectedOutcomeId = 3;

			var getDecisionResponse = _fixture.Create<GetDecisionResponse>();

			getDecisionResponse.Outcome.Status = DecisionOutcomeStatus.PartiallyApproved;

			getDecisionResponse.Outcome.DecisionMadeDate = new DateTimeOffset(new DateTime(2022, 05, 02));
			getDecisionResponse.Outcome.DecisionEffectiveFromDate = new DateTimeOffset(new DateTime(2022, 06, 16));

			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecision(expectedCaseUrn, expectedDecisionId)).ReturnsAsync(getDecisionResponse);

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedCaseUrn)
				.WithDecisionId(expectedDecisionId)
				.WithOutcomeId(expectedOutcomeId)
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync();

			sut.ViewData[ViewDataConstants.Title].Should().Be("Edit outcome");

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.DecisionOutcome.Status.Should().Be(getDecisionResponse.Outcome.Status);
			sut.DecisionOutcome.TotalAmount.Should().Be(getDecisionResponse.Outcome.TotalAmount);
			sut.DecisionOutcome.Authorizer.Should().Be(getDecisionResponse.Outcome.Authorizer);
			sut.DecisionOutcome.BusinessAreasConsulted.Should().BeEquivalentTo(getDecisionResponse.Outcome.BusinessAreasConsulted);

			sut.DecisionOutcome.DecisionMadeDate.Day.Should().Be("2");
			sut.DecisionOutcome.DecisionMadeDate.Month.Should().Be("5");
			sut.DecisionOutcome.DecisionMadeDate.Year.Should().Be("2022");

			sut.DecisionOutcome.DecisionEffectiveFromDate.Day.Should().Be("16");
			sut.DecisionOutcome.DecisionEffectiveFromDate.Month.Should().Be("6");
			sut.DecisionOutcome.DecisionEffectiveFromDate.Year.Should().Be("2022");
			sut.SaveAndContinueButtonText.Should().Be("Save and return to decision");
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecisionOutcomeDoesNotExist_Then_ThrowsException()
		{
			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecision(2, 1)).Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(1)
				.WithDecisionId(2)
				.WithOutcomeId(3)
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync();

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		[Test]
		public async Task OnPostAsync_When_NewDecisionOutcome_Returns_PageRedirectToCase()
		{
			const int expectedUrn = 2;
			const int expectedDecisionId = 3;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithDecisionId(expectedDecisionId)
				.BuildSut();

			sut.DecisionMadeDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.DecisionMadeDate.Date = new OptionalDateModel()
			{
				Day = "22",
				Month = "05",
				Year = "2022"
			};

			sut.DecisionEffectiveFromDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.DecisionEffectiveFromDate.Date = new OptionalDateModel()
			{
				Day = "04",
				Month = "11",
				Year = "2022"
			};

			var page = await sut.OnPostAsync() as RedirectResult;

			sut.CaseUrn.Should().Be(expectedUrn);
			sut.DecisionId.Should().Be(expectedDecisionId);
			page.Url.Should().Be("/case/2/management");
			sut.DecisionOutcome.BusinessAreasConsulted.Should().BeEmpty();
			sut.DecisionOutcome.DecisionMadeDate.Should().NotBeNull();
			sut.DecisionOutcome.DecisionEffectiveFromDate.Should().NotBeNull();
		}

		[Test]
		public async Task OnPostAsync_When_ExistingDecisionOutcome_Returns_PageRedirectToDecision()
		{
			const int expectedUrn = 2;
			const int expectedDecisionId = 3;
			const int expectedOutcomeId = 4;
			var builder = new TestBuilder();

			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithDecisionId(expectedDecisionId)
				.WithOutcomeId(expectedOutcomeId)
				.BuildSut();

			sut.DecisionOutcome.BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea> {
				DecisionOutcomeBusinessArea.Funding
			};

			sut.DecisionMadeDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.DecisionMadeDate.Date = new OptionalDateModel()
			{
				Day = "23",
				Month = "11",
				Year = "2022"
			};

			sut.DecisionEffectiveFromDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.DecisionEffectiveFromDate.Date = new OptionalDateModel()
			{
				Day = "24",
				Month = "11",
				Year = "2022"
			};

			var page = await sut.OnPostAsync() as RedirectResult;

			page.Url.Should().Be("/case/2/management/action/decision/3");
			sut.DecisionOutcome.BusinessAreasConsulted.Should().Contain(DecisionOutcomeBusinessArea.Funding);
			sut.DecisionOutcome.DecisionMadeDate.Should().NotBeNull();
			sut.DecisionOutcome.DecisionEffectiveFromDate.Should().NotBeNull();
		}

		private class TestBuilder
		{
			private Mock<IDecisionService> _mockDecisionService;
			private readonly Mock<ILogger<AddPageModel>> _mockLogger;
			private readonly bool _isAuthenticated;
			private int _caseUrnValue;
			private int _decisionId;
			private int? _outcomeId;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());

				_isAuthenticated = true;

				_mockDecisionService = Fixture.Freeze<Mock<IDecisionService>>();
				_mockLogger = Fixture.Freeze<Mock<ILogger<AddPageModel>>>();
			}

			public TestBuilder WithCaseUrn(int urnValue)
			{
				_caseUrnValue = urnValue;

				return this;
			}

			public TestBuilder WithDecisionId(int decisionId)
			{
				_decisionId = decisionId;

				return this;
			}

			public TestBuilder WithOutcomeId(int outcomeId)
			{
				_outcomeId = outcomeId;

				return this;
			}

			public TestBuilder WithDecisionService(Mock<IDecisionService> decisionService)
			{
				_mockDecisionService = decisionService;

				return this;
			}

			public AddPageModel BuildSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(_isAuthenticated);

				var result = new AddPageModel(_mockDecisionService.Object, _mockLogger.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata,
					DecisionOutcome = new EditDecisionOutcomeModel(),
					DecisionOutcomeStatus = _fixture.Create<RadioButtonsUiComponent>(),
					DecisionOutcomeAuthorizer = _fixture.Create<RadioButtonsUiComponent>()
				};

				result.CaseUrn = _caseUrnValue;
				result.DecisionId = _decisionId;

				if (_outcomeId.HasValue)
				{
					result.OutcomeId = _outcomeId.Value;
				}

				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}

			public Fixture Fixture { get; set; }
		}
	}
}

