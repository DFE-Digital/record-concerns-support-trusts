﻿using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using AutoFixture;
using System.Linq;
using FluentAssertions;
using ConcernsCaseWork.API.Contracts.Srma;

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
			Assert.That(validationError, Is.EqualTo("Close SRMA action"));
		}

		[Test]
		public void When_No_Open_SRMA_Return_Empty_String()
		{
			//arrange
			var closedAt = DateTime.Now;
			var srmaValidator = new SRMAValidator();
			var srmaModels = SrmaFactory.BuildListSrmaModel(SRMAStatus.Unknown, SRMAReasonOffered.Unknown, closedAt);

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
			Assert.That(validationError, Is.EqualTo("Close financial plan"));
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
			Assert.That(validationError, Is.EqualTo("Close NTI: Under consideration"));
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
			Assert.That(validationError, Is.EqualTo("Close NTI: Warning letter"));
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
			Assert.That(validationError, Is.EqualTo("Cancel, lift or close NTI: Notice to improve"));
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

			result.Should().Be("Close decisions");
		}

		[Test]
		public void When_NoOpenTrustFinancialForecast_Returns_EmptyString()
		{
			var input = _fixture.CreateMany<TrustFinancialForecastSummaryModel>().ToList();

			var validator = new TrustFinancialForecastValidator();

			var result = validator.Validate(input);

			result.Should().BeEmpty();
		}

		[Test]
		public void When_OpenTrustFinancialForecast_Returns_ValidationError()
		{
			var input = _fixture.CreateMany<TrustFinancialForecastSummaryModel>().ToList();
			input.ForEach(c => c.ClosedAt = null);

			var validator = new TrustFinancialForecastValidator();

			var result = validator.Validate(input);

			result.Should().Be("Close trust financial forecast");
		}

		[Test]
		public void When_NoOpenTargetedTrustEngagement_Returns_EmptyString()
		{
			var input = _fixture.CreateMany<TargetedTrustEngagmentModel>().ToList();

			var validator = new TargetedTrustEngagementValidator();

			var result = validator.Validate(input);

			result.Should().BeEmpty();
		}

		[Test]
		public void When_OpenTargetedTrustEngagement_Returns_ValidationError()
		{
			var input = _fixture.CreateMany<TargetedTrustEngagmentModel>().ToList();
			input.ForEach(c => c.ClosedAt = null);

			var validator = new TargetedTrustEngagementValidator();

			var result = validator.Validate(input);

			result.Should().Be("Close TTE (targeted trust engagement)");
		}
	}
}