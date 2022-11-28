using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
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
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision.Outcome
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class AddPageModelTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public void AddPageModel_Is_AbstractPageModel()
		{
			var builder = new TestBuilder();
			var sut = builder.BuildSut();
			Assert.NotNull(sut as AbstractPageModel);
		}

		[Test]
		public async Task OnGetAsync_When_NewDecisionOutcome_Returns_Page()
		{
			const long expectedUrn = 2;
			const long expectedDecisionId = 7;
			var builder = new TestBuilder();
			var sut = builder
				.BuildSut();

			await sut.OnGetAsync(expectedUrn, expectedDecisionId);

			sut.ViewData[ViewDataConstants.Title].Should().Be("Add outcome");

			var expectedDecision = new CreateDecisionOutcomeRequest()
			{
				BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea> { }
			};

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.DecisionOutcome.Should().BeEquivalentTo(expectedDecision);

			sut.CaseUrn.Should().Be(expectedUrn);
			sut.DecisionId.Should().Be(expectedDecisionId);
			sut.DecisionOutcomesCheckBoxes.Should().NotBeEmpty();
			sut.BusinessAreaCheckBoxes.Should().NotBeEmpty();
			sut.AuthoriserCheckBoxes.Should().NotBeEmpty();
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecision_Returns_Page()
		{
			const long expectedCaseUrn = 1;
			const long expectedDecisionId = 2;
			const long expectedOutcomeId = 3;

			var getDecisionOutcomeResponse = _fixture.Create<GetDecisionOutcomeResponse>();

			getDecisionOutcomeResponse.DecisionOutcomeStatus = DecisionOutcomeStatus.PartiallyApproved;

			getDecisionOutcomeResponse.DecisionMadeDate = new DateTimeOffset(new DateTime(2022, 5, 2));
			getDecisionOutcomeResponse.DecisionTakeEffectDate = new DateTimeOffset(new DateTime(2022, 6, 16));

			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecisionOutcome(expectedDecisionId, expectedOutcomeId)).ReturnsAsync(getDecisionOutcomeResponse);

			var builder = new TestBuilder();
			var sut = builder
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync(expectedCaseUrn, expectedDecisionId, expectedOutcomeId);

			sut.ViewData[ViewDataConstants.Title].Should().Be("Edit outcome");

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.DecisionOutcome.Status.Should().Be(getDecisionOutcomeResponse.DecisionOutcomeStatus);
			sut.DecisionOutcome.TotalAmount.Should().Be(getDecisionOutcomeResponse.TotalAmountApproved);
			sut.DecisionOutcome.Authorizer.Should().Be(getDecisionOutcomeResponse.Authoriser);
			sut.DecisionOutcome.BusinessAreasConsulted.Should().BeEquivalentTo(getDecisionOutcomeResponse.BusinessAreasConsulted);

			sut.DecisionMadeDate.Day.Should().Be("02");
			sut.DecisionMadeDate.Month.Should().Be("05");
			sut.DecisionMadeDate.Year.Should().Be("2022");

			sut.DecisionTakeEffectDate.Day.Should().Be("16");
			sut.DecisionTakeEffectDate.Month.Should().Be("06");
			sut.DecisionTakeEffectDate.Year.Should().Be("2022");
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecisionOutcomeDoesNotExist_Then_ThrowsException()
		{
			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecisionOutcome(2, 1)).Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync(1, 2, 3);

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		[Test]
		public async Task OnPostAsync_When_NewDecisionOutcome_Returns_PageRedirectToCase()
		{
			const long expectedUrn = 2;
			const long expectedDecisionId = 3;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.DecisionMadeDate = new OptionalDateModel()
			{
				Day = "22",
				Month = "05",
				Year = "2022"
			};

			sut.DecisionTakeEffectDate = new OptionalDateModel()
			{
				Day = "04",
				Month = "11",
				Year = "2022"
			};

			var page = await sut.OnPostAsync(expectedUrn, expectedDecisionId) as RedirectResult;

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
			const long expectedUrn = 2;
			const long expectedDecisionId = 3;
			const long expectedOutcomeId = 4;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.DecisionOutcome.BusinessAreasConsulted = new List<DecisionOutcomeBusinessArea> {
				DecisionOutcomeBusinessArea.Funding
			};

			sut.DecisionMadeDate = new OptionalDateModel()
			{
				Day = "23",
				Month = "11",
				Year = "2022"
			};

			sut.DecisionTakeEffectDate = new OptionalDateModel()
			{
				Day = "24",
				Month = "11",
				Year = "2022"
			};

			var page = await sut.OnPostAsync(expectedUrn, expectedDecisionId, expectedOutcomeId) as RedirectResult;

			page.Url.Should().Be("/case/2/management/action/decision/3");
			sut.DecisionOutcome.BusinessAreasConsulted.Should().Contain(DecisionOutcomeBusinessArea.Funding);
			sut.DecisionOutcome.DecisionMadeDate.Should().NotBeNull();
			sut.DecisionOutcome.DecisionEffectiveFromDate.Should().NotBeNull();
		}

		[Test]
		public async Task OnPostAsync_When_DateIsNotSet_Returns_MinDate()
		{
			const long expectedUrn = 2;
			const long expectedDecisionId = 3;

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.DecisionMadeDate = new OptionalDateModel();
			sut.DecisionTakeEffectDate = new OptionalDateModel();

			var page = await sut.OnPostAsync(expectedUrn, expectedDecisionId);

			sut.DecisionOutcome.DecisionMadeDate.Should().Be(DateTime.MinValue);
			sut.DecisionOutcome.DecisionEffectiveFromDate.Should().Be(DateTime.MinValue);
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();
		}

		[Test]
		public async Task OnPostAsync_When_DateFieldsArePopulated_Then_PopulateCreateDecisionDateProperty_Returns_Page()
		{
			const long expectedUrn = 2;
			const long expectedDecisionId = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.DecisionTakeEffectDate = new OptionalDateModel()
			{
				Day = "23",
				Month = "11",
				Year = "2022"
			};

			sut.DecisionMadeDate = new OptionalDateModel()
			{
				Day = "24",
				Month = "11",
				Year = "2022"
			};

			await sut.OnPostAsync(expectedUrn, expectedDecisionId);

			Assert.IsNotNull(sut.DecisionOutcome.DecisionEffectiveFromDate);
			Assert.IsNotNull(sut.DecisionOutcome.DecisionMadeDate);
		}

		[Test]
		public async Task OnPostAsync_When_Validation_Failures_Concatenates_Error_Messages()
		{
			long expectedUrn = 233433;
			long expectedDecision = 434;
			const string expectedMessage1 = "Select a decision outcome";
			const string expectedMessage2 = "Please enter a complete date DD MM YYYY";

			var builder = new TestBuilder();

			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.ModelState.AddModelError("Error1", expectedMessage1);
			sut.ModelState.AddModelError("Error2", expectedMessage2);

			await sut.OnPostAsync(expectedUrn, expectedDecision);

			var decisionsValidationErrors = sut.TempData["Decision.Message"] as IEnumerable<string>;

			Assert.IsTrue(decisionsValidationErrors.Contains(expectedMessage1));
			Assert.IsTrue(decisionsValidationErrors.Contains(expectedMessage2));
			sut.BusinessAreaCheckBoxes.Should().NotBeEmpty();
			sut.ViewData[ViewDataConstants.Title].Should().Be("Add outcome");
		}

		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		public async Task OnPostAsync_When_InvalidCaseUrnRouteValue_Then_Throws_Exception(long caseUrn)
		{
			var decisionId = 123;
			var builder = new TestBuilder()
				.WithCaseUrnRouteValue(caseUrn);

			var sut = builder.BuildSut();

			await sut.OnPostAsync(caseUrn, decisionId);

			Assert.AreEqual(AddPageModel.ErrorOnPostPage, sut.TempData["Error.Message"]);
		}

		private class TestBuilder
		{
			private Mock<IDecisionService> _mockDecisionService;
			private readonly Mock<ILogger<AddPageModel>> _mockLogger;
			private readonly bool _isAuthenticated;
			private object _caseUrnValue;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());

				_isAuthenticated = true;

				_mockDecisionService = Fixture.Freeze<Mock<IDecisionService>>();
				_mockLogger = Fixture.Freeze<Mock<ILogger<AddPageModel>>>();
			}

			public TestBuilder WithCaseUrnRouteValue(object urnValue)
			{
				_caseUrnValue = urnValue;

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
					DecisionOutcome = new EditDecisionOutcomeModel()
				};

				var routeData = result.RouteData.Values;
				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}

			public Fixture Fixture { get; set; }
		}
	}
}

