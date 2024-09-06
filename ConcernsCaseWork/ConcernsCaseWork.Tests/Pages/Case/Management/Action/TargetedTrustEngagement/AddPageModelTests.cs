using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;

using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement;
using ConcernsCaseWork.Service.TargetedTrustEngagement;
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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.TargetedTrustEngagement
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class AddPageModelTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public async Task OnGetAsync_When_NewTargetedTrustEngagement_Returns_Page()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			await sut.OnGetAsync();

			sut.ViewData[ViewDataConstants.Title].Should().Be("Add TTE");
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();
			sut.TargetedTrustEngagement.CaseUrn.Should().Be((int)expectedUrn);
			sut.CaseUrn.Should().Be(expectedUrn);
			sut.SaveAndContinueButtonText.Should().Be("Save and return to case overview");
		}

		[Test]
		public async Task OnGetAsync_When_ExistingTTE_Returns_Page()
		{
			const int expectedUrn = 2;
			const int expectedTTEId = 1;

			var getTTEResponse = _fixture.Create<GetTargetedTrustEngagementResponse>();

			getTTEResponse.ActivityId = TargetedTrustEngagementActivity.BudgetForecastReturnAccountsReturnDriven;
			getTTEResponse.ActivityTypes = new TargetedTrustEngagementActivityType[] 
			{
				TargetedTrustEngagementActivityType.Category1
			};
			getTTEResponse.EngagementStartDate = new DateTimeOffset(new DateTime(2024, 9, 3));

			var tteService = new Mock<ITargetedTrustEngagementService>();
			tteService.Setup(m => m.GetTargetedTrustEngagement(2, 1)).ReturnsAsync(getTTEResponse);

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.WithTTEId(expectedTTEId)
				.WithTargetedTrustEngagementService(tteService)
				.BuildSut();

			await sut.OnGetAsync();

			sut.ViewData[ViewDataConstants.Title].Should().Be("Edit TTE");

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.TargetedTrustEngagement.CaseUrn.Should().Be(getTTEResponse.CaseUrn);
			sut.TargetedTrustEngagement.ActivityId.Should().Be(getTTEResponse.ActivityId);
			sut.TargetedTrustEngagement.ActivityTypes.Should().BeEquivalentTo(getTTEResponse.ActivityTypes);
			sut.TargetedTrustEngagement.Notes.Should().Be(getTTEResponse.Notes);
			sut.TargetedTrustEngagement.EngagementStartDate.Should().Be(getTTEResponse.EngagementStartDate);
			sut.EngagementStartDate.Date.Day.Should().Be("03");
			sut.EngagementStartDate.Date.Month.Should().Be("09");
			sut.EngagementStartDate.Date.Year.Should().Be("2024");
			sut.SaveAndContinueButtonText.Should().Be("Save and return to engagement");
		}

		[Test]
		public async Task OnGetAsync_When_ExistingTTEDoesNotExist_Then_ThrowsException()
		{
			var tteService = new Mock<ITargetedTrustEngagementService>();
			tteService.Setup(m => m.GetTargetedTrustEngagement(2, 1)).Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(1)
				.WithTTEId(2)
				.WithTargetedTrustEngagementService(tteService)
				.BuildSut();

			await sut.OnGetAsync();

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		[Test]
		public async Task OnPostAsync_When_NewTTE_Returns_PageRedirectToCase()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.EngagementStartDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.EngagementStartDate.Date = new OptionalDateModel()
			{
				Day = "03",
				Month = "09",
				Year = "2024"
			};

			sut.EngagementActivitiesComponent.SelectedId = (int)TargetedTrustEngagementActivity.BudgetForecastReturnAccountsReturnDriven;
			sut.EngagementActivitiesComponent.SelectedSubId = (int)TargetedTrustEngagementActivityType.Category1;

			sut.Notes = _fixture.Create<TextAreaUiComponent>();

			var page = await sut.OnPostAsync() as RedirectResult;

			sut.CaseUrn.Should().Be(expectedUrn);
			page.Url.Should().Be("/case/2/management");
			sut.TargetedTrustEngagement.ActivityId.Should().Be(TargetedTrustEngagementActivity.BudgetForecastReturnAccountsReturnDriven);
			sut.TargetedTrustEngagement.ActivityTypes.Should().NotBeEmpty();
			sut.TargetedTrustEngagement.EngagementStartDate.Should().NotBeNull();
		}

		[Test]
		public async Task OnPostAsync_When_ExistingTTE_Returns_PageRedirectToTTE()
		{
			const int expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.WithTTEId(1)
				.BuildSut();

			sut.EngagementStartDate = _fixture.Create<OptionalDateTimeUiComponent>();
			sut.EngagementStartDate.Date = new OptionalDateModel()
			{
				Day = "03",
				Month = "09",
				Year = "2024"
			};

			sut.EngagementActivitiesComponent.SelectedId = (int)TargetedTrustEngagementActivity.BudgetForecastReturnAccountsReturnDriven;
			sut.EngagementActivitiesComponent.SelectedSubId = (int)TargetedTrustEngagementActivityType.Category1;

			sut.Notes = _fixture.Create<TextAreaUiComponent>();

			var page = await sut.OnPostAsync() as RedirectResult;

			page.Url.Should().Be("/case/2/management/action/targetedtrustengagement/1");
			sut.TargetedTrustEngagement.ActivityId.Should().Be(TargetedTrustEngagementActivity.BudgetForecastReturnAccountsReturnDriven);
			sut.TargetedTrustEngagement.ActivityTypes.Should().Contain(x => x == TargetedTrustEngagementActivityType.Category1);
			sut.TargetedTrustEngagement.EngagementStartDate.Should().NotBeNull();
		}

		private class TestBuilder
		{
			private Mock<ITargetedTrustEngagementService> _targetedTrustEngagementService;
			private readonly Mock<ILogger<AddPageModel>> _mockLogger;
			private readonly bool _isAuthenticated;
			private int _caseUrnValue;
			private int? _targetedTrustEngagementId;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());
				
				_isAuthenticated = true;
				
				_caseUrnValue = 5;
				_targetedTrustEngagementService = Fixture.Freeze<Mock<ITargetedTrustEngagementService>>();
				_mockLogger = Fixture.Freeze<Mock<ILogger<AddPageModel>>>();
			}

			public TestBuilder WithCaseUrnRouteValue(int urnValue)
			{
				_caseUrnValue = urnValue;

				return this;
			}

			public TestBuilder WithTTEId(int tteId)
			{
				_targetedTrustEngagementId = tteId;

				return this;
			}

			public TestBuilder WithTargetedTrustEngagementService(Mock<ITargetedTrustEngagementService> targetedTrustEngagementService) 
			{
				_targetedTrustEngagementService = targetedTrustEngagementService;

				return this;
			}

			public AddPageModel BuildSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(_isAuthenticated);


				var result = new AddPageModel(_targetedTrustEngagementService.Object, _mockLogger.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata,
					TargetedTrustEngagement = new CreateTargetedTrustEngagementRequest()
				};

				result.TargetedTrustEngagement = _fixture.Create<CreateTargetedTrustEngagementRequest>();
				result.EngagementActivitiesComponent = _fixture.Create<RadioButtonsUiComponent>();

				result.CaseUrn = _caseUrnValue;

				if (_targetedTrustEngagementId.HasValue) result.TargetedTrustEngagementId = _targetedTrustEngagementId;

				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}
			
			public Fixture Fixture { get; set; }
		}
	}
}