using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
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

		private class TestBuilder
		{
			private Mock<IDecisionService> _mockDecisionService;
			private readonly Mock<ILogger<AddPageModel>> _mockLogger;
			private readonly bool _isAuthenticated;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());

				_isAuthenticated = true;

				_mockDecisionService = Fixture.Freeze<Mock<IDecisionService>>();
				_mockLogger = Fixture.Freeze<Mock<ILogger<AddPageModel>>>();
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
					DecisionOutcome = new CreateDecisionOutcomeRequest()
				};

				var routeData = result.RouteData.Values;
				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}

			public Fixture Fixture { get; set; }
		}
	}
}

