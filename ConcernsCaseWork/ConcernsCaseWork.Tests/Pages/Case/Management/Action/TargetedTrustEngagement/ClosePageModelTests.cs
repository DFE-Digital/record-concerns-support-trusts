using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement;
using ConcernsCaseWork.Service.Decision;
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.TargetedTrustEngagement
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class ClosePageModelTests
	{
		private readonly static Fixture _fixture = new();
		
		[Test]
		public async Task OnGetAsync_When_TTE_Found_Returns_Page()
		{
			// arrange
			const int expectedUrn = 2;
			const int expectedTTEId = 1;
			
			var tteService = new Mock<ITargetedTrustEngagementService>();
			
			var tte = CreateOpenTTEResponse(expectedUrn);
			tteService.Setup(m => m.GetTargetedTrustEngagement(expectedUrn, expectedTTEId)).ReturnsAsync(tte);
			
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithTTEId(expectedTTEId)
				.WithNotes(tte.Notes)
				.WithTTEService(tteService)
				.BuildSut();

			// act
			await sut.OnGetAsync();

			// assert
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();
			sut.CaseUrn.Should().Be(expectedUrn);
			sut.TargetedtrustengagementId.Should().Be(expectedTTEId);
			sut.Notes.Text.StringContents.Should().Be(tte.Notes);
		}

		[Test]
		public async Task OnGetAsync_When_TTE_Is_Closed_Redirects_To_TTE_View_Page()
		{
			// arrange
			const int expectedUrn = 2;
			const int expectedTTEId = 1;

			var tteService = new Mock<ITargetedTrustEngagementService>();

			var tte = CreateClosedTTEResponse(expectedUrn);
			tteService.Setup(m => m.GetTargetedTrustEngagement(expectedUrn, expectedTTEId)).ReturnsAsync(tte);

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithTTEId(expectedTTEId)
				.WithTTEService(tteService)
				.BuildSut();

			// act
			var page = await sut.OnGetAsync();

			// assert
			page.Should().BeOfType<RedirectResult>();
			sut.CaseUrn.Should().Be(expectedUrn);
			((RedirectResult)page).Url.Should().Be($"/case/{expectedUrn}/management/action/targetedtrustengagement/{expectedTTEId}");
		}

		[Test]
		public async Task OnGetAsync_When_GetTTEThrowsException_Then_SetsError()
		{
			// arrange
			var tteService = new Mock<ITargetedTrustEngagementService>();
			tteService.Setup(m => m.GetTargetedTrustEngagement(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithTTEService(tteService)
				.BuildSut();

			// act
			await sut.OnGetAsync();

			// assert
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		[Test]
		public async Task OnPostAsync_When_NewTTE_Returns_PageRedirectToCase()
		{
			// arrange
			const int expectedUrn = 2;
			const int expectedTTEId = 2;
			const int outcomeId = 1;
			DateTime expectedDate = DateTime.Now;

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithTTEId(expectedTTEId)
				.WithNotes("valid notes")
				.WithOutcome(outcomeId)
				.WithDate(expectedDate)
				.BuildSut();

			// act
			var page = await sut.OnPostAsync() as RedirectResult;

			// assert
			sut.CaseUrn.Should().Be(expectedUrn);
			page!.Url.Should().Be("/case/2/management");
		}

		[Test]
		public async Task OnPostAsync_When_CloseTTE_Throws_Error_Returns_Error()
		{
			// arrange
			var tteService = new Mock<ITargetedTrustEngagementService>();
			tteService
				.Setup(m => m.CloseTargetedTrustEngagement(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CloseTargetedTrustEngagementRequest>()))
					.Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithTTEService(tteService)
				.BuildSut();

			// act
			var page = await sut.OnPostAsync();

			// assert
			page.Should().BeOfType<PageResult>();
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnPostPage);
		}

		[Test]
		public async Task OnPostAsync_When_ModelValidationErrorThrown_Returns_Page()
		{
			// arrange
			var builder = new TestBuilder();
			var tteService = new Mock<ITargetedTrustEngagementService>();

			var sut = builder
				.WithTTEService(tteService)
				.BuildSut();

			sut.ModelState.AddModelError("someErrorProperty", "Some error message");

			// act
			var result = await sut.OnPostAsync();

			// assert
			result.Should().BeOfType<PageResult>();

			tteService.Verify(s => s.GetTargetedTrustEngagement(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		private static GetTargetedTrustEngagementResponse CreateOpenTTEResponse(int caseUrn)
			=> _fixture
				.Build<GetTargetedTrustEngagementResponse>()
				.With(r => r.ClosedAt, (DateTimeOffset?)null)
				.With(r => r.CaseUrn, caseUrn)
				.Create();
		
		private static GetTargetedTrustEngagementResponse CreateClosedTTEResponse(int caseUrn)
			=> _fixture
				.Build<GetTargetedTrustEngagementResponse>()
				.With(r => r.ClosedAt, DateTime.Now)
				.With(r => r.CaseUrn, caseUrn)
				.Create();

		private class TestBuilder
		{
			private Mock<ITargetedTrustEngagementService> _mockTargetedTrustEngagementService;
			private readonly Mock<ILogger<ClosePageModel>> _mockLogger;
			private readonly bool _isAuthenticated;
			private int _caseUrnValue;
			private int _tteId;
			private int _outcomeId;
			private string _notes;
			private DateTime _date;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());
				
				_isAuthenticated = true;

				_mockTargetedTrustEngagementService = Fixture.Freeze<Mock<ITargetedTrustEngagementService>>();
				_mockLogger = Fixture.Freeze<Mock<ILogger<ClosePageModel>>>();
			}

			public TestBuilder WithCaseUrn(int urnValue)
			{
				_caseUrnValue = urnValue;

				return this;
			}
			
			public TestBuilder WithTTEId(int tteId)
			{
				_tteId = tteId;

				return this;
			}

			public TestBuilder WithOutcome(int outcome)
			{
				_outcomeId = outcome;

				return this;
			}

			public TestBuilder WithDate(DateTime date)
			{
				_date = date;

				return this;
			}

			public TestBuilder WithNotes(string notes)
			{
				_notes = notes;

				return this;
			}

			public TestBuilder WithTTEService(Mock<ITargetedTrustEngagementService> tteService) 
			{
				_mockTargetedTrustEngagementService = tteService;

				return this;
			}

			public ClosePageModel BuildSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(_isAuthenticated);

				var result = new ClosePageModel(_mockTargetedTrustEngagementService.Object, _mockLogger.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata,
					CaseUrn = _caseUrnValue,
					TargetedtrustengagementId = _tteId,
					EngagementOutcomeComponent = _fixture.Create<RadioButtonsUiComponent>(),
					EngagementEndDate = _fixture.Create<OptionalDateTimeUiComponent>(),
					Notes = _fixture.Create<TextAreaUiComponent>()
				};

				result.Notes.Text.StringContents = _notes;
				result.EngagementOutcomeComponent.SelectedId = _outcomeId;
				result.EngagementEndDate.Date.Day = _date.Day.ToString();
				result.EngagementEndDate.Date.Month = _date.Month.ToString();
				result.EngagementEndDate.Date.Year = _date.Year.ToString();

				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}
			
			public Fixture Fixture { get; set; }
		}
	}
}