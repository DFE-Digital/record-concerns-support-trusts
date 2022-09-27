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

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision
{
	[Parallelizable(ParallelScope.All)]
	public class AddPageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnPage()
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

			await sut.OnGetAsync();
			
			Assert.AreEqual(expectedUrn, sut.CaseUrn);
		}

		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(null)]
		[TestCase("")]
		public async Task OnGetAsync_When_InvalidCaseUrnRouteValue_Then_Throws_Exception(object caseUrn)
		{
			var builder = new TestBuilder()
				.WithCaseUrnRouteValue(caseUrn);

			var sut = builder.BuildSut();

			await sut.OnGetAsync();

			Assert.AreEqual(AddPageModel.ErrorOnGetPage, sut.TempData["Error.Message"]);
		}

		private class TestBuilder
		{
			private readonly Mock<ILogger<AddPageModel>> _mockLogger;
			private readonly bool _isAuthenticated;
			private object _caseUrnValue;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());
				
				_isAuthenticated = true;
				
				_caseUrnValue = 5;
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

				var result = new AddPageModel(_mockLogger.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata
				};

				var routeData = result.RouteData.Values;
				routeData.Add("urn", _caseUrnValue);

				return result;
			}
			

			public Fixture Fixture { get; set; }
		}
	}
}