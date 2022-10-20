using AutoFixture;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Pages.Case.Management.Action.Decision;
using ConcernsCaseWork.Pages.Base;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.Metrics;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Service.TRAMS.Decision;
using ConcernsCaseWork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sentry;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public void AddPageModel_Is_AbstractPageModel()
		{
			var builder = new TestBuilder();
			var sut = builder.BuildSut();
			Assert.NotNull(sut as AbstractPageModel);
		}

		[Test]
		public async Task OnGetAsync_Returns_Page()
		{
			const long expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			await sut.OnGetAsync(expectedUrn);
			
			Assert.AreEqual(expectedUrn, sut.CaseUrn);
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
		public async Task OnPostAsync_Returns_Page()
		{
			const long expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			await sut.OnPostAsync(expectedUrn);

			Assert.AreEqual(expectedUrn, sut.CaseUrn);
		}


		[Test]
		public async Task OnPostAsync_When_DecisionTypesIsPopulated_Then_PopulateCreateDecisionTypesProperty_Returns_Page()
		{
			const long expectedUrn = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrnRouteValue(expectedUrn)
				.BuildSut();

			sut.DecisionTypeNoticeToImprove = true;
			sut.DecisionTypeOtherFinancialSupport = true;
			sut.DecisionTypeQualifiedFloatingCharge = true;

			const int expectedDecisionTypesLength = 3;


			await sut.OnPostAsync(expectedUrn);

			Assert.AreEqual(sut.CreateDecisionDto.DecisionTypes.Length, expectedDecisionTypesLength);
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

			Assert.IsNotNull(sut.CreateDecisionDto.ReceivedRequestDate);
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

		private class TestBuilder
		{
			private readonly Mock<IDecisionService> _mockDecisionService;
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

			public AddPageModel BuildSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(_isAuthenticated);

				var result = new AddPageModel(_mockDecisionService.Object, _mockLogger.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata,
					CreateDecisionDto = new CreateDecisionDto()
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