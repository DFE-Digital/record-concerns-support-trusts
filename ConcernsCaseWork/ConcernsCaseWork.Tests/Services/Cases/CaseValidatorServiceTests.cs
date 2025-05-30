﻿using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Decisions;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Service.Decision;
using ConcernsCaseWork.Service.TargetedTrustEngagement;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.Nti;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;
using ConcernsCaseWork.Services.Records;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Cases
{
	public class CaseValidatorServiceTests
	{
		[Test]
		public async Task When_HasNoOpenActionsOrConcerns_Returns_NoValidationMessages()
		{
			var validator = CreateService();

			var result = await validator.ValidateClose(1);

			result.Should().BeEmpty();
		}

		[Test]
		public async Task When_HasClosedConcern_Returns_NoValidationMessages()
		{
			var recordsModelService = new Mock<IRecordModelService>();
			recordsModelService.Setup(m => m.GetRecordsModelByCaseUrn(1)).ReturnsAsync(
				new List<RecordModel>() { new RecordModel() { StatusId = (long)CaseStatus.Close} });

			var validator = CreateService(recordsModelService);

			var result = await validator.ValidateClose(1);

			result.Should().BeEmpty();
		}

		[Test]
		public async Task When_HasOpenActionsAndConcerns_Returns_ValidationMessages()
		{
			var recordsModelService = new Mock<IRecordModelService>();
			recordsModelService.Setup(m => m.GetRecordsModelByCaseUrn(1)).ReturnsAsync(
				new List<RecordModel>() { new RecordModel() { StatusId = (long)CaseStatus.Live } });

			var caseActionValidator = new Mock<ICaseActionValidator>();
			caseActionValidator.Setup(m => m.Validate(It.IsAny<List<CaseActionModel>>())).Returns(new List<string>() { "Case Action Error" });

			var validator = CreateService(
				recordsModelService,
				caseActionValidator);

			var result = await validator.ValidateClose(1);

			var expected = new List<CaseValidationErrorModel>()
			{
				new CaseValidationErrorModel() { Type = CaseValidationError.Concern, Error = "Close concerns" },
				new CaseValidationErrorModel() { Type = CaseValidationError.CaseAction, Error = "Case Action Error" },

			};

			result.Should().BeEquivalentTo(expected);
		}

		private ICaseValidatorService CreateService(
			Mock<IRecordModelService> recordsModelService = null, 
			Mock<ICaseActionValidator> caseActionValidator = null)
		{
			if (recordsModelService == null)
			{
				recordsModelService = new Mock<IRecordModelService>();
				recordsModelService.Setup(m => m.GetRecordsModelByCaseUrn(1)).ReturnsAsync(new List<RecordModel>() { });
			}

			if (caseActionValidator == null)
			{
				caseActionValidator = new Mock<ICaseActionValidator>();
				caseActionValidator.Setup(m => m.Validate(It.IsAny<List<CaseActionModel>>())).Returns(new List<string>());
			}

			var srmaService = new Mock<ISRMAService>();
			srmaService.Setup(m => m.GetSRMAsForCase(1)).ReturnsAsync(new List<SRMAModel>());

			var financialPlanService = new Mock<IFinancialPlanModelService>();
			financialPlanService.Setup(m => m.GetFinancialPlansModelByCaseUrn(1)).ReturnsAsync(new List<FinancialPlanModel>());

			var ntiUnderConsiderationService = new Mock<INtiUnderConsiderationModelService>();
			ntiUnderConsiderationService.Setup(m => m.GetNtiUnderConsiderationsForCase(1)).ReturnsAsync(new List<NtiUnderConsiderationModel>());

			var ntiWarningLetterService = new Mock<INtiWarningLetterModelService>();
			ntiWarningLetterService.Setup(m => m.GetNtiWarningLettersForCase(1)).ReturnsAsync(new List<NtiWarningLetterModel>());

			var ntiService = new Mock<INtiModelService>();
			ntiService.Setup(m => m.GetNtisForCaseAsync(1)).ReturnsAsync(new List<NtiModel>());

			var decisionService = new Mock<IDecisionService>();
			decisionService.Setup(m => m.GetDecisionsByCaseUrn(1)).ReturnsAsync(new List<DecisionSummaryResponse>());

			var trustFinancialForecastService = new Mock<ITrustFinancialForecastService>();
			trustFinancialForecastService.Setup(m => m.GetAllForCase(1)).ReturnsAsync(new List<TrustFinancialForecastResponse>());

			var targetedTrustEngagementService = new Mock<ITargetedTrustEngagementService>();
			targetedTrustEngagementService.Setup(m => m.GetTargetedTrustEngagementByCaseUrn(1)).ReturnsAsync([]);

			var result = new CaseValidatorService(
				recordsModelService.Object,
				srmaService.Object,
				financialPlanService.Object,
				ntiUnderConsiderationService.Object,
				ntiWarningLetterService.Object,
				ntiService.Object,
				decisionService.Object,
				trustFinancialForecastService.Object,
				targetedTrustEngagementService.Object,
				caseActionValidator.Object);

			return result;
		}
	}
}
