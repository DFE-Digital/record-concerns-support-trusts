using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.Decision;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Services.Cases;
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
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class AddPageModelTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public async Task OnGetAsync_When_NewDecision_Returns_Page()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			await sut.OnGetAsync();

			sut.ViewData[ViewDataConstants.Title].Should().Be("Add decision");

			var expectedDecision = new CreateDecisionRequest()
			{
				DecisionTypes = new DecisionTypeQuestion[] { }
			};

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.Decision.Should().BeEquivalentTo(expectedDecision, options => 
				options.Excluding(o => o.ConcernsCaseUrn));
			sut.Decision.ConcernsCaseUrn.Should().Be((int)expectedUrn);

			sut.CaseUrn.Should().Be(expectedUrn);
			sut.DecisionTypeQuestions.Should().NotBeEmpty();
			sut.SaveAndContinueButtonText.Should().Be("Save and return to case overview");
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecision_Returns_Page()
		{
			const int expectedUrn = 2;
			const int expectedDecisionId = 1;

			var getDecisionResponse = _fixture.Create<GetDecisionResponse>();
			getDecisionResponse.DecisionTypes = new DecisionTypeQuestion[] {

				new DecisionTypeQuestion()
				{
					Id = DecisionType.NoticeToImprove
				},
				new DecisionTypeQuestion()
				{
					Id = DecisionType.OtherFinancialSupport
				}
			};
			getDecisionResponse.ReceivedRequestDate = new DateTimeOffset(new DateTime(2022, 5, 2));

			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecision(2, 1)).ReturnsAsync(getDecisionResponse);

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.WithDecisionId(expectedDecisionId)
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync();

			sut.ViewData[ViewDataConstants.Title].Should().Be("Edit decision");

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.Decision.ConcernsCaseUrn.Should().Be(getDecisionResponse.ConcernsCaseUrn);
			sut.Decision.CrmCaseNumber.Should().Be(getDecisionResponse.CrmCaseNumber);
			sut.Decision.DecisionTypes.Should().BeEquivalentTo(getDecisionResponse.DecisionTypes);
			sut.Decision.SupportingNotes.Should().Be(getDecisionResponse.SupportingNotes);
			sut.ReceivedRequestDate.Date.Day.Should().Be("02");
			sut.ReceivedRequestDate.Date.Month.Should().Be("05");
			sut.ReceivedRequestDate.Date.Year.Should().Be("2022");
			sut.SaveAndContinueButtonText.Should().Be("Save and return to decision");
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecisionDoesNotExist_Then_ThrowsException()
		{
			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecision(2, 1)).Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(1)
				.WithDecisionId(2)
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync();

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		[Test]
		public async Task OnPostAsync_When_NewDecision_Returns_PageRedirectToCase()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.ReceivedRequestDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.ReceivedRequestDate.Date = new OptionalDateModel()
			{
				Day = "22",
				Month = "05",
				Year = "2022"
			};

			sut.DecisionTypeQuestions = new List<DecisionTypeQuestionModel>();

			sut.Notes = _fixture.Create<TextAreaUiComponent>();

			var page = await sut.OnPostAsync() as RedirectResult;

			sut.CaseUrn.Should().Be(expectedUrn);
			page.Url.Should().Be("/case/2/management");
			sut.Decision.DecisionTypes.Should().BeEmpty();
			sut.Decision.ReceivedRequestDate.Should().NotBeNull();
		}

		[Test]
		public async Task OnPostAsync_When_ExistingDecision_Returns_PageRedirectToDecision()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.WithDecisionId(1)
				.BuildSut();

		

			sut.ReceivedRequestDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.ReceivedRequestDate.Date = new OptionalDateModel()
			{
				Day = "22",
				Month = "05",
				Year = "2022"
			};

			sut.DecisionTypeQuestions = new List<DecisionTypeQuestionModel> { new DecisionTypeQuestionModel() { Id = DecisionType.NonRepayableFinancialSupport } };

            sut.Notes = _fixture.Create<TextAreaUiComponent>();

			var page = await sut.OnPostAsync() as RedirectResult;

			page.Url.Should().Be("/case/2/management/action/decision/1");
			sut.Decision.DecisionTypes.Should().Contain(x => x.Id == DecisionType.NonRepayableFinancialSupport);
			sut.Decision.ReceivedRequestDate.Should().NotBeNull();
		}

		[Test]
		public async Task OnPostAsync_When_DateIsNotSet_Returns_MinDate()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.ReceivedRequestDate = _fixture.Create <OptionalDateTimeUiComponent>();
			sut.ReceivedRequestDate.Date = new OptionalDateModel();
			sut.Notes = _fixture.Create<TextAreaUiComponent>();
			sut.DecisionTypeQuestions = new List<DecisionTypeQuestionModel>();

			var page = await sut.OnPostAsync();

			sut.Decision.ReceivedRequestDate.Should().Be(DateTime.MinValue);
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();
		}


		[Test]
		public async Task OnPostAsync_When_DateFieldsArePopulated_Then_PopulateCreateDecisionDateProperty_Returns_Page()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.ReceivedRequestDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.ReceivedRequestDate.Date = new OptionalDateModel()
			{
				Day = "19",
				Month = "10",
				Year = "2022"
			};

			sut.Notes = _fixture.Create<TextAreaUiComponent>();

			await sut.OnPostAsync();

			Assert.IsNotNull(sut.Decision.ReceivedRequestDate);
		}

		private class TestBuilder
		{
			private Mock<IDecisionService> _mockDecisionService;
			private readonly Mock<ILogger<AddPageModel>> _mockLogger;
			private readonly bool _isAuthenticated;
			private int _caseUrnValue;
			private int? _decisionId;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());
				
				_isAuthenticated = true;
				
				_caseUrnValue = 5;
				_mockDecisionService = Fixture.Freeze<Mock<IDecisionService>>();
				_mockLogger = Fixture.Freeze<Mock<ILogger<AddPageModel>>>();
			}

			public TestBuilder WithCaseUrnRouteValue(int urnValue)
			{
				_caseUrnValue = urnValue;

				return this;
			}

			public TestBuilder WithDecisionId(int decisionId)
			{
				_decisionId = decisionId;

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

				var caseModel = _fixture.Create<CaseModel>();
				caseModel.Urn = _caseUrnValue;
				var caseModelService = new Mock<ICaseModelService>();
				caseModelService.Setup(m => m.GetCaseByUrn(It.IsAny<int>())).ReturnsAsync(caseModel);

				var result = new AddPageModel(_mockDecisionService.Object, caseModelService.Object, _mockLogger.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata,
					Decision = new CreateDecisionRequest()
				};

				result.IsSubmissionRequired = _fixture.Create<RadioButtonsUiComponent>();
				result.HasCrmCase = _fixture.Create<RadioButtonsUiComponent>();
				result.RetrospectiveApproval = _fixture.Create<RadioButtonsUiComponent>();

				result.CaseUrn = _caseUrnValue;

				if (_decisionId.HasValue) result.DecisionId = _decisionId;

				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}
			
			public Fixture Fixture { get; set; }
		}
	}
}