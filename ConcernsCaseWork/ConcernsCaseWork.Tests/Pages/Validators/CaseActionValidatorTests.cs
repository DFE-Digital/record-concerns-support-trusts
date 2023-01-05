using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using AutoFixture;
using System.Linq;
using FluentAssertions;

namespace ConcernsCaseWork.Tests.Pages.Validators
{
	[Parallelizable(ParallelScope.All)]
	public class CaseActionValidatorTests
	{
		private readonly Fixture _fixture = new();

		[Test]
		public void When_Open_SRMA_Return_Error_Message()
		{
			//arrange 
			var srmaValidator = new SRMAValidator();
			var srmaModels = SrmaFactory.BuildListSrmaModel();

			// act
			string validationError = srmaValidator.Validate(srmaModels);

			// assert
			Assert.That(validationError, Is.EqualTo("Resolve SRMA"));
		}

		[Test]
		public void When_No_Open_SRMA_Return_Empty_String()
		{
			//arrange
			var closedAt = DateTime.Now;
			var srmaValidator = new SRMAValidator();
			var srmaModels = SrmaFactory.BuildListSrmaModel(Enums.SRMAStatus.Unknown, Enums.SRMAReasonOffered.Unknown, closedAt);

			// act
			string validationError = srmaValidator.Validate(srmaModels);

			// assert
			Assert.That(validationError, Is.EqualTo(string.Empty));
		}

		[Test]
		public void When_Open_Financial_Plan_Return_Error_Message()
		{
			//arrange 
			var financialPlanValidator = new FinancialPanValidator();
			var financialPlanModels = FinancialPlanFactory.BuildListFinancialPlanModel();

			// act
			string validationError = financialPlanValidator.Validate(financialPlanModels);

			// assert
			Assert.That(validationError, Is.EqualTo("Resolve Financial Plan"));
		}

		[Test]
		public void When_No_Open_Financial_Plan_Return_Empty_String()
		{
			//arrange
			var closedAt = DateTime.Now;
			var financialPlanValidator = new FinancialPanValidator();
			var financialPlanModels = FinancialPlanFactory.BuildListFinancialPlanModel(closedAt);

			// act
			string validationError = financialPlanValidator.Validate(financialPlanModels);

			// assert
			Assert.That(validationError, Is.EqualTo(string.Empty));
		}

		[Test]
		public void When_Open_NTI_UC_Return_Error_Message()
		{
			//arrange 
			var ntiUCValidator = new NTIUnderConsiderationValidator();
			var ntiUCModels = NTIUnderConsiderationFactory.BuildListNTIUnderConsiderationModel();

			// act
			string validationError = ntiUCValidator.Validate(ntiUCModels);

			// assert
			Assert.That(validationError, Is.EqualTo("Resolve NTI Under Consideration"));
		}

		[Test]
		public void When_No_Open_NTI_UC_Return_Empty_String()
		{
			//arrange
			var closedAt = DateTime.Now;
			var ntiUCValidator = new NTIUnderConsiderationValidator();
			var ntiUCModels = NTIUnderConsiderationFactory.BuildClosedListNTIUnderConsiderationModel();

			// act
			string validationError = ntiUCValidator.Validate(ntiUCModels);

			// assert
			Assert.That(validationError, Is.EqualTo(string.Empty));
		}

		[Test]
		public void When_Open_NTI_WL_Return_Error_Message()
		{
			//arrange 
			var ntiWLValidator = new NTIWarningLetterValidator();
			var ntiWLModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3);

			// act
			string validationError = ntiWLValidator.Validate(ntiWLModels);

			// assert
			Assert.That(validationError, Is.EqualTo("Resolve NTI Warning Letter"));
		}

		[Test]
		public void When_No_Open_NTI_WL_Return_Empty_Stringe()
		{
			//arrange
			var closedAt = DateTime.Now;
			var ntiWLValidator = new NTIWarningLetterValidator();
			var ntiWLModels = NTIWarningLetterFactory.BuildListNTIWarningLetterModels(3, closedAt);

			// act
			string validationError = ntiWLValidator.Validate(ntiWLModels);

			// assert
			Assert.That(validationError, Is.EqualTo(string.Empty));
		}

		[Test]
		public void When_Open_NTI_Return_Error_Message()
		{
			//arrange 
			var ntiValidator = new NTIValidator();
			var ntiModels = NTIFactory.BuildListNTIModel();

			// act
			string validationError = ntiValidator.Validate(ntiModels);

			// assert
			Assert.That(validationError, Is.EqualTo("Resolve Notice To Improve"));
		}

		[Test]
		public void When_No_Open_NTI_Return_Empty_String()
		{
			//arrange
			var closedAt = DateTime.Now;
			var ntiValidator = new NTIValidator();
			var ntiModels = NTIFactory.BuildClosedListNTIModel();

			// act
			string validationError = ntiValidator.Validate(ntiModels);

			// assert
			Assert.That(validationError, Is.EqualTo(string.Empty));
		}

		[Test]
		public void When_NoOpenDecisions_Returns_EmptyString()
		{
			var caseActionModels = _fixture.CreateMany<CaseActionModel>().ToList();
			caseActionModels.ForEach(c => c.ClosedAt = null);

			var decisionModels = _fixture.CreateMany<DecisionSummaryModel>().ToList();

			var input = new List<CaseActionModel>();
			input.AddRange(caseActionModels);
			input.AddRange(decisionModels);

			var validator = new DecisionValidator();

			var result = validator.Validate(input);

			result.Should().BeEmpty();
		}

		[Test]
		public void When_MultipleOpenDecisions_Returns_ErrorMessage()
		{
			var input = _fixture.CreateMany<DecisionSummaryModel>().ToList();
			input.ForEach(c => c.ClosedAt = null);

			var validator = new DecisionValidator();

			var result = validator.Validate(input);

			result.Should().Be("Resolve Decision(s)");
		}
	}
}