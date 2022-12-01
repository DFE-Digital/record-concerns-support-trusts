using AutoFixture;
using AutoFixture.AutoMoq;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.Decisions;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models.Validatable;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Case.Management.Action.Decision;
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
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RedirectResult = Microsoft.AspNetCore.Mvc.RedirectResult;

namespace ConcernsCaseWork.Tests.Pages.Case.Management.Action.Decision
{
	[Parallelizable(ParallelScope.Fixtures)]
	public class ClosePageModelTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public void ClosePageModel_Is_AbstractPageModel()
		{
			var builder = new TestBuilder();
			var sut = builder.BuildSut();
			sut.Should().BeAssignableTo<AbstractPageModel>();
		}
		
		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		public void OnValidateModel_When_InvalidCaseUrn_ShouldFailModelValidation(int caseUrn)
		{
			// arrange
			var builder = new TestBuilder()
				.WithCaseUrn(caseUrn)
				.WithDecisionId(1);

			// act
			var sut = builder.BuildSut();

			// assert
			SimpleValidator.IsModelValid(sut).Should().BeFalse();
			var validationResults = SimpleValidator.Validate(sut).Results;
			validationResults.Count.Should().Be(1);
			validationResults.Single().ErrorMessage.Should().Be("CaseUrn must be provided");
			validationResults.Single().MemberNames.Should().Contain("CaseUrn");
		}
		
		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		public void OnValidateModel_When_InvalidDecisionId_ShouldFailModelValidation(int decisionId)
		{
			// arrange
			var builder = new TestBuilder()
				.WithCaseUrn(1)
				.WithDecisionId(decisionId);

			// act
			var sut = builder.BuildSut();

			// assert
			SimpleValidator.IsModelValid(sut).Should().BeFalse();
			var validationResults = SimpleValidator.Validate(sut).Results;
			validationResults.Count.Should().Be(1);
			validationResults.Single().ErrorMessage.Should().Be("DecisionId must be provided");
			validationResults.Single().MemberNames.Should().Contain("DecisionId");
		}
		
		[Test]
		[TestCase(2001)]
		[TestCase(3000)]
		public void OnValidateModel_When_NotesTooLong_ShouldFailModelValidation(int notesLength)
		{
			// arrange
			var builder = new TestBuilder()
				.WithCaseUrn(1)
				.WithDecisionId(1)
				.WithNotes("notes too long".PadRight(notesLength, 'a'));

			// act
			var sut = builder.BuildSut();

			// assert
			SimpleValidator.IsModelValid(sut).Should().BeFalse();
			var validationResults = SimpleValidator.Validate(sut).Results;
			validationResults.Count.Should().Be(1);
			validationResults.Single().ErrorMessage.Should().Be("Supporting Notes must be 2000 characters or less");
			validationResults.Single().MemberNames.Should().Contain("Notes");
		}

		[Test]
		public async Task OnGetAsync_When_Decision_Found_Returns_Page()
		{
			// arrange
			const int expectedUrn = 2;
			const int expectedDecisionId = 1;
			
			var decisionService = new Mock<IDecisionService>();
			
			var decision = CreateOpenDecisionResponse(expectedUrn);
			decisionService.Setup(m => m.GetDecision(expectedUrn, expectedDecisionId)).ReturnsAsync(decision);
			
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithDecisionId(expectedDecisionId)
				.WithDecisionService(decisionService)
				.BuildSut();

			// act
			await sut.OnGetAsync();

			// assert
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().BeNull();
			sut.CaseUrn.Should().Be(expectedUrn);
			sut.DecisionId.Should().Be(expectedDecisionId);
			sut.Notes.Should().Be(decision.SupportingNotes);
		}
		
		[Test]
		public async Task OnGetAsync_When_ModelValidationErrorThrown_Returns_Page()
		{
			// arrange
			var builder = new TestBuilder();
			var decisionService = new Mock<IDecisionService>();
			
			var sut = builder
				.WithDecisionService(decisionService)
				.BuildSut();
			
			sut.ModelState.AddModelError("someErrorProperty", "Some error message");

			// act
			var result = await sut.OnGetAsync();
			
			// assert
			result.Should().BeOfType<PageResult>();
			
			decisionService.Verify(s => s.GetDecision(It.IsAny<long>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task OnGetAsync_When_GetDecisionThrowsException_Then_SetsError()
		{
			// arrange
			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecision(It.IsAny<long>(), It.IsAny<int>())).Throws(new Exception("Failed"));

			var builder = new TestBuilder();
			var sut = builder
				.WithDecisionService(decisionService)
				.BuildSut();

			// act
			await sut.OnGetAsync();

			// assert
			sut.TempData[ErrorConstants.ErrorMessageKey].Should().Be(ErrorConstants.ErrorOnGetPage);
		}

		[Test]
		public async Task OnPostAsync_When_NewDecision_Returns_PageRedirectToCase()
		{
			// arrange
			const int expectedUrn = 2;
			const int expectedDecisionId = 2;
			var builder = new TestBuilder();
			var sut = builder
				.WithCaseUrn(expectedUrn)
				.WithDecisionId(expectedDecisionId)
				.WithNotes("valid notes".PadRight(2000, 'a'))
				.BuildSut();

			// act
			var page = await sut.OnPostAsync() as RedirectResult;

			// assert
			sut.CaseUrn.Should().Be(expectedUrn);
			page!.Url.Should().Be("/case/2/management");
		}
		
		[Test]
		public async Task OnPostAsync_When_ModelValidationErrorThrown_Returns_Page()
		{
			// arrange
			var builder = new TestBuilder();
			var decisionService = new Mock<IDecisionService>();
			
			var sut = builder
				.WithDecisionService(decisionService)
				.BuildSut();
			
			sut.ModelState.AddModelError("someErrorProperty", "Some error message");

			// act
			var result = await sut.OnPostAsync();
			
			// assert
			result.Should().BeOfType<PageResult>();
			
			decisionService.Verify(s => s.GetDecision(It.IsAny<long>(), It.IsAny<int>()), Times.Never);
		}
		
		private static GetDecisionResponse CreateOpenDecisionResponse(int caseUrn)
			=> _fixture
				.Build<GetDecisionResponse>()
				.With(r => r.ClosedAt, (DateTimeOffset?)null)
				.With(r => r.ConcernsCaseUrn, caseUrn)
				.Create();

		private class TestBuilder
		{
			private Mock<IDecisionService> _mockDecisionService;
			private readonly Mock<ILogger<ClosePageModel>> _mockLogger;
			private readonly bool _isAuthenticated;
			private int _caseUrnValue;
			private int _decisionId;
			private string _notes;

			public TestBuilder()
			{
				this.Fixture = new Fixture();
				this.Fixture.Customize(new AutoMoqCustomization());
				
				_isAuthenticated = true;
				
				_mockDecisionService = Fixture.Freeze<Mock<IDecisionService>>();
				_mockLogger = Fixture.Freeze<Mock<ILogger<ClosePageModel>>>();
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
						
			public TestBuilder WithNotes(string notes)
			{
				_notes = notes;

				return this;
			}

			public TestBuilder WithDecisionService(Mock<IDecisionService> decisionService) 
			{
				_mockDecisionService = decisionService;

				return this;
			}

			public ClosePageModel BuildSut()
			{
				(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(_isAuthenticated);

				var result = new ClosePageModel(_mockDecisionService.Object, _mockLogger.Object)
				{
					PageContext = pageContext,
					TempData = tempData,
					Url = new UrlHelper(actionContext),
					MetadataProvider = pageContext.ViewData.ModelMetadata,
					CaseUrn = _caseUrnValue,
					DecisionId = _decisionId,
					Notes = _notes
				};
				
				result.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

				return result;
			}
			
			public Fixture Fixture { get; set; }
		}
	}
	
	public static class SimpleValidator
	{
		/// <summary>
		/// Validate the model and return a response, which includes any validation messages and an IsValid bit.
		/// </summary>
		public static ValidationResponse Validate(object model)
		{
			var results = new List<ValidationResult>();
			var context = new ValidationContext(model);

			var isValid = Validator.TryValidateObject(model, context, results, true);
         
			return new ValidationResponse()
			{
				IsValid = isValid,
				Results = results
			};
		}

		/// <summary>
		/// Validate the model and return a bit indicating whether the model is valid or not.
		/// </summary>
		public static bool IsModelValid(object model)
		{
			var response = Validate(model);

			return response.IsValid;
		}
	}

	public class ValidationResponse
	{
		public List<ValidationResult> Results { get; set; }
		public bool IsValid { get; set; }

		public ValidationResponse()
		{
			Results = new List<ValidationResult>();
			IsValid = false;
		}
	}
}