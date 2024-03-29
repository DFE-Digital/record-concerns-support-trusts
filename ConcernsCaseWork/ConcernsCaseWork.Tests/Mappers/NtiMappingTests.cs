﻿using AutoFixture;
using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Nti;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class NtiMappingTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public void WhenMapDtoToServiceModel_ReturnsCorrectModel()
		{
			//arrange
			var testData = new
			{
				Id = 1L,
				CaseUrn = 123,
				CreatedAt = DateTime.Now.AddDays(-5),
				Notes = "Test notes",
				Reasons = new[] { NtiReason.Safeguarding },
				DateStarted = DateTime.Now.AddDays(-1),
				Status = new KeyValuePair<int, string>(1, "Status 1"),
				UpdatedAt = DateTime.Now.AddDays(-1),
				SumissionDecisionId = "1000001",
				DateNTIClosed = DateTime.Now.AddDays(-3),
				DateNTILifted = DateTime.Now.AddDays(-4)

			};

			var ntiDto = new NtiDto
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = null,
				Notes = testData.Notes,
				DateStarted = testData.DateStarted,
				StatusId = (int)NtiStatus.Lifted,
				UpdatedAt = testData.UpdatedAt,
				ReasonsMapping = testData.Reasons.Select(r => (int)r).ToArray(),
				SumissionDecisionId = testData.SumissionDecisionId,
				DateNTIClosed = testData.DateNTIClosed,
				DateNTILifted = testData.DateNTILifted
			};

			var casePermissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };

			// act
			var serviceModel = NtiMappers.ToServiceModel(ntiDto, casePermissionsResponse);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
			Assert.That(serviceModel.Id, Is.EqualTo(testData.Id));
			Assert.That(serviceModel.Reasons, Is.Not.Null);
			Assert.That(serviceModel.Reasons.Count, Is.EqualTo(testData.Reasons.Length));
			Assert.That(serviceModel.Reasons.ElementAt(0), Is.EqualTo(testData.Reasons.ElementAt(0)));
			Assert.That(serviceModel.Status, Is.EqualTo(NtiStatus.Lifted));
			Assert.That(serviceModel.SubmissionDecisionId, Is.EqualTo(testData.SumissionDecisionId));
			Assert.That(serviceModel.DateNTILifted, Is.EqualTo(testData.DateNTILifted));
			Assert.That(serviceModel.DateNTIClosed, Is.EqualTo(testData.DateNTIClosed));

			serviceModel.IsEditable.Should().BeTrue();
		}

		[TestCaseSource(nameof(GetPermissionTestCases))]
		public void WhenMapDtoToServiceModel_Not_Editable_ReturnsCorrectModel(
			DateTime closedDate,
			GetCasePermissionsResponse casePermissionsResponse)
		{
			var ntiDto = _fixture.Create<NtiDto>();
			ntiDto.StatusId = null;
			ntiDto.ClosedStatusId = null;
			ntiDto.ClosedAt = closedDate;

			var serviceModel = NtiMappers.ToServiceModel(ntiDto, casePermissionsResponse);

			serviceModel.IsEditable.Should().BeFalse();
			serviceModel.ClosedAt.Should().Be(closedDate);
		}

		[Test]
		public void WhenMapDtoToDbModel_ReturnsCorrectModel()
		{
			//arrange
			var testData = new
			{
				Id = 1L,
				CaseUrn = 123L,
				CreatedAt = DateTime.Now.AddDays(-5),
				ClosedAt = DateTime.Now,
				Notes = "Test notes",
				Reasons = new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(1, "Reason1") },
				DateStarted = DateTime.Now.AddDays(-1),
				Status = NtiStatus.PreparingNTI,
				UpdatedAt = DateTime.Now.AddDays(-1),
				SumissionDecisionId = "1000001",
				DateNTIClosed = DateTime.Now.AddDays(-3),
				DateNTILifted = DateTime.Now.AddDays(-4)
			};

			var serviceModel = new NtiModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				Notes = testData.Notes,
				Reasons = new[] { NtiReason.CashFlowProblems },
				Status = NtiStatus.PreparingNTI,
				DateStarted = testData.DateStarted,
				UpdatedAt = testData.UpdatedAt,
				SubmissionDecisionId = testData.SumissionDecisionId,
				DateNTIClosed = testData.DateNTIClosed,
				DateNTILifted = testData.DateNTILifted
			};

			// act
			var dbModel = NtiMappers.ToDBModel(serviceModel);

			// assert
			Assert.That(dbModel, Is.Not.Null);
			Assert.That(dbModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
			Assert.That(dbModel.Id, Is.EqualTo(testData.Id));
			Assert.That(dbModel.StatusId, Is.Not.Null);
			Assert.That(dbModel.StatusId, Is.EqualTo((int)testData.Status));
			Assert.That(dbModel.Notes, Is.EqualTo(testData.Notes));
			Assert.That(dbModel.ReasonsMapping.First(), Is.EqualTo(testData.Reasons.First().Key));
			Assert.That(dbModel.DateStarted, Is.EqualTo(testData.DateStarted));
			Assert.That(dbModel.UpdatedAt, Is.EqualTo(testData.UpdatedAt));
			Assert.That(serviceModel.SubmissionDecisionId, Is.EqualTo(testData.SumissionDecisionId));
			Assert.That(serviceModel.DateNTILifted, Is.EqualTo(testData.DateNTILifted));
			Assert.That(serviceModel.DateNTIClosed, Is.EqualTo(testData.DateNTIClosed));
		}
		
		[Test]
		public void WhenMapDbModelToActionSummary_ReturnsCorrectModel()
		{
			//arrange
			var testData = new
			{
				Id = _fixture.Create<int>(),
				CaseUrn = _fixture.Create<int>(),
				CreatedAt = _fixture.Create<DateTime>(),
				ClosedAt = _fixture.Create<DateTime>(),
			};

			var serviceModel = new NtiModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				ClosedStatusId = NtiStatus.Cancelled
			};

			// act
			var actionSummary = serviceModel.ToActionSummary();
			
			// assert
			Assert.Multiple(() =>
			{
				Assert.That(actionSummary.Name, Is.EqualTo("NTI"));
				Assert.That(actionSummary.ClosedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.ClosedAt)));
				Assert.That(actionSummary.OpenedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.CreatedAt)));
				Assert.That(actionSummary.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/nti/{testData.Id}"));
				Assert.That(actionSummary.StatusName, Is.EqualTo("Cancelled"));
			});

			actionSummary.RawOpenedDate.Should().Be(testData.CreatedAt);
			actionSummary.RawClosedDate.Should().Be(testData.ClosedAt);
		}

		[TestCaseSource(nameof(GetStatusTestCases))]
		public void WhenMapDbModelToActionSummary_WhenActionIsOpen_ReturnsCorrectStatus(
			NtiStatus? status,
			string expectedResult)
		{
			var model = _fixture.Create<NtiModel>();
			model.ClosedAt = null;
			model.Status = status;

			var result = model.ToActionSummary();

			result.StatusName.Should().Be(expectedResult);
		}

		private static IEnumerable<TestCaseData> GetStatusTestCases()
		{
			yield return new TestCaseData(null, "In progress");
			 yield return new TestCaseData(NtiStatus.PreparingNTI, "Preparing NTI");
		}

		private static IEnumerable<TestCaseData> GetPermissionTestCases()
		{
			yield return new TestCaseData(new DateTime(), new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
			yield return new TestCaseData(null, new GetCasePermissionsResponse());
		}
	}
}