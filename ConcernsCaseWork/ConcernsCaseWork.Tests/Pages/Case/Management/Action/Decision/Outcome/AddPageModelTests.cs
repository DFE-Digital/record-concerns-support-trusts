using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.Enums;
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
				BusinessAreasConsulted = new BusinessArea[] { }
			};

			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();

			sut.DecisionOutcome.Should().BeEquivalentTo(expectedDecision, options =>
				options.Excluding(o => o.DecisionId));

			sut.DecisionOutcome.DecisionId.Should().Be((int)expectedDecisionId);
			sut.CaseUrn.Should().Be(expectedUrn);
			sut.DecisionId.Should().Be(expectedDecisionId);
			sut.DecisionOutcomesCheckBoxes.Should().NotBeEmpty();
			sut.BusinessAreaCheckBoxes.Should().NotBeEmpty();
			sut.AuthoriserCheckBoxes.Should().NotBeEmpty();
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

