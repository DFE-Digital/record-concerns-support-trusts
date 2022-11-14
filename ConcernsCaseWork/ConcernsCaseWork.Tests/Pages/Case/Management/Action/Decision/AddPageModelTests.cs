﻿using AutoFixture;
using NUnit.Framework;
using System.Threading.Tasks;
using ConcernsCaseWork.Pages.Case.Management.Action.Decision;
using ConcernsCaseWork.Pages.Base;
using Microsoft.Extensions.Logging;
using Moq;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ConcernsCaseWork.Service.Decision;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using FluentAssertions;
using NUnit.Framework.Constraints;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using System;
using ConcernsCaseWork.Constants;
using Azure;
using ConcernsCaseWork.API.Contracts.Enums;
using System.Transactions;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision
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
		public async Task OnGetAsync_When_NewDecision_Returns_Page()
		{
			const long expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			await sut.OnGetAsync(expectedUrn);

			sut.ViewData[ViewDataConstants.Title].Should().Be("Add decision");

			var expectedDecision = new CreateDecisionRequest()
			{
				DecisionTypes = new DecisionType[] { }
			};

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.Decision.Should().BeEquivalentTo(expectedDecision, options => 
				options.Excluding(o => o.ConcernsCaseUrn));
			sut.Decision.ConcernsCaseUrn.Should().Be((int)expectedUrn);

			sut.CaseUrn.Should().Be(expectedUrn);
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecision_Returns_Page()
		{
			const long expectedUrn = 2;
			const long expectedDecisionId = 1;

			var getDecisionResponse = _fixture.Create<GetDecisionResponse>();
			getDecisionResponse.DecisionTypes = new API.Contracts.Enums.DecisionType[] {
				API.Contracts.Enums.DecisionType.NoticeToImprove,
				API.Contracts.Enums.DecisionType.OtherFinancialSupport
			};

			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecision(2, 1)).ReturnsAsync(getDecisionResponse);

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync(expectedUrn, expectedDecisionId);

			sut.ViewData[ViewDataConstants.Title].Should().Be("Edit decision");

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.Decision.ConcernsCaseUrn.Should().Be(getDecisionResponse.ConcernsCaseUrn);
			sut.Decision.CrmCaseNumber.Should().Be(getDecisionResponse.CrmCaseNumber);
			sut.Decision.DecisionTypes.Should().BeEquivalentTo(getDecisionResponse.DecisionTypes);
			sut.Decision.SupportingNotes.Should().Be(getDecisionResponse.SupportingNotes);
		}

		[Test]
		public async Task OnGetAsync_When_ExistingDecisionDoesNotExist_Then_ThrowsException()
		{
			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecision(2, 1)).Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(1)
				.WithDecisionService(decisionService)
				.BuildSut();

			await sut.OnGetAsync(1, 2);

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		public async Task OnGetAsync_When_InvalidCaseUrnRouteValue_Then_Throws_Exception(long caseUrn)
		{
			var builder = new TestBuilder()
				.WithCaseUrnRouteValue(caseUrn);

			var sut = builder.BuildSut();

			await sut.OnGetAsync(caseUrn);

			Assert.AreEqual(AddPageModel.ErrorOnGetPage, sut.TempData["Error.Message"]);
		}

		[Test]
		public async Task OnPostAsync_When_NewDecision_Returns_PageRedirectToCase()
		{
			const long expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			var page = await sut.OnPostAsync(expectedUrn) as RedirectResult;

			sut.CaseUrn.Should().Be(expectedUrn);
			page.Url.Should().Be("/case/2/management");
			sut.Decision.DecisionTypes.Should().BeEmpty();
		}

		[Test]
		public async Task OnPostAsync_When_ExistingDecision_Returns_PageRedirectToDecision()
		{
			const long expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.Decision.DecisionTypes = new DecisionType[] {
				DecisionType.NonRepayableFinancialSupport
			};

			var page = await sut.OnPostAsync(expectedUrn, 1) as RedirectResult;

			page.Url.Should().Be("/case/2/management/action/decision/1");
			sut.Decision.DecisionTypes.Should().Contain(DecisionType.NonRepayableFinancialSupport);
		}


		[Test]
		public async Task OnPostAsync_When_DateFieldsArePopulated_Then_PopulateCreateDecisionDateProperty_Returns_Page()
		{
			const long expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-request-received", new StringValues("19") },
					{ "dtr-month-request-received", new StringValues("10") },
					{ "dtr-year-request-received", new StringValues("2022") }
				});

			await sut.OnPostAsync(expectedUrn);

			Assert.IsNotNull(sut.Decision.ReceivedRequestDate);
		}


		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		public async Task OnPostAsync_When_InvalidCaseUrnRouteValue_Then_Throws_Exception(long caseUrn)
		{
			var builder = new TestBuilder()
				.WithCaseUrnRouteValue(caseUrn);

			var sut = builder.BuildSut();

			await sut.OnPostAsync(caseUrn);

			Assert.AreEqual(AddPageModel.ErrorOnPostPage, sut.TempData["Error.Message"]);
		}

		[Test]
		public async Task OnPostAsync_When_Validation_Failures_Concatenates_Error_Messages()
		{
			long caseUrn = 233433;
			const string expectedMessage1 = "Notes must be 2000 characters or less";
			const string expectedMessage2 = "Submission document link must be 2048 or less";

			var builder = new TestBuilder()
				.WithCaseUrnRouteValue(caseUrn);

			var sut = builder.BuildSut();

			sut.ModelState.AddModelError("Error1", expectedMessage1);
			sut.ModelState.AddModelError("Error2", expectedMessage2);

			await sut.OnPostAsync(caseUrn);

			var decisionsValidationErrors = sut.TempData["Decision.Message"] as IEnumerable<string>;

			Assert.IsTrue(decisionsValidationErrors.Contains(expectedMessage1));
			Assert.IsTrue(decisionsValidationErrors.Contains(expectedMessage2));
		}

		[Test]
		public async Task OnPostAsync_When_InvalidDate_Error_Message()
		{
			long caseUrn = 233433;
			var invalidDay = "32";
			var invalidMonth = "13";
			var invalidYear = "7";

			var expectedErrorMessage = $"{invalidDay}-{invalidMonth}-{invalidYear} is an invalid date";

			var builder = new TestBuilder()
				.WithCaseUrnRouteValue(caseUrn);

			var sut = builder.BuildSut();

			sut.HttpContext.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>
				{
					{ "dtr-day-request-received", new StringValues(invalidDay) },
					{ "dtr-month-request-received", new StringValues(invalidMonth) },
					{ "dtr-year-request-received", new StringValues(invalidYear) }
				});


			await sut.OnPostAsync(caseUrn);

			var decisionsValidationErrors = sut.TempData["Decision.Message"] as IEnumerable<string>;

			Assert.IsTrue(decisionsValidationErrors.Contains(expectedErrorMessage));
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
				
				_caseUrnValue = 5;
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
					Decision = new CreateDecisionRequest()
				};

				var routeData = result.RouteData.Values;
				routeData.Add("urn", _caseUrnValue);
				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}
			
			public Fixture Fixture { get; set; }
		}
	}
}