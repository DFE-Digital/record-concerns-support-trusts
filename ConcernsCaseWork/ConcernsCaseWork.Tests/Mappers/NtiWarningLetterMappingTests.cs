using AutoFixture;
using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Shared.Tests.Factory;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class NtiWarningLetterMappingTests
	{
		private readonly Fixture _fixture = new();

		[Test]
		public void WhenMapDtoToServiceModel_ReturnsCorrectModel()
		{
			//arrange
			var testData = new
			{
				Id = 1L,
				CaseUrn = 123,
				CreatedAt = DateTime.Now.AddDays(-5),
				ClosedAt = DateTime.Now,
				Notes = "Test notes",
				Reasons = new[] { NtiWarningLetterReason.CashFlowProblems },
				SentDate = DateTime.Now.AddDays(-1),
				Status = 1,
				UpdatedAt = DateTime.Now.AddDays(-1),
				ClosedStatusId = 3
			};

			var ntiDto = new NtiWarningLetterDto
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = null,
				Notes = testData.Notes,
				DateLetterSent = testData.SentDate,
				StatusId = 1,
				UpdatedAt = testData.UpdatedAt,
				WarningLetterReasonsMapping = new[] { 1 },
				ClosedStatusId = testData.ClosedStatusId
			};

			var permissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };

			// act
			var serviceModel = NtiWarningLetterMappers.ToServiceModel(ntiDto, permissionsResponse);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
			Assert.That(serviceModel.Id, Is.EqualTo(testData.Id));
			Assert.That(serviceModel.Reasons, Is.Not.Null);
			Assert.That(serviceModel.Reasons.Count, Is.EqualTo(testData.Reasons.Length));
			Assert.That(serviceModel.Reasons.ElementAt(0), Is.EqualTo(testData.Reasons.ElementAt(0)));
			Assert.That(serviceModel.Status, Is.EqualTo((NtiWarningLetterStatus?)testData.Status));
			Assert.That(serviceModel.ClosedStatusId, Is.EqualTo((NtiWarningLetterStatus?)testData.ClosedStatusId));
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
				Reasons = new[] { NtiWarningLetterReason.CashFlowProblems },
				SentDate = DateTime.Now.AddDays(-1),
				Status = NtiWarningLetterStatus.SentToTrust,
				UpdatedAt = DateTime.Now.AddDays(-1)
			};

			var serviceModel = new NtiWarningLetterModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				Notes = testData.Notes,
				Reasons = new NtiWarningLetterReason[] { NtiWarningLetterReason.CashFlowProblems },
				Status = NtiWarningLetterStatus.SentToTrust,
				SentDate = testData.SentDate,
				UpdatedAt = testData.UpdatedAt
			};

			// act
			var dbModel = NtiWarningLetterMappers.ToDBModel(serviceModel);

			// assert
			Assert.That(dbModel, Is.Not.Null);
			Assert.That(dbModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
			Assert.That(dbModel.Id, Is.EqualTo(testData.Id));

			Assert.That(dbModel.StatusId, Is.Not.Null);
			Assert.That(dbModel.StatusId, Is.EqualTo((int?)testData.Status));
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
				Status = NtiWarningLetterStatus.SentToTrust
			};

			var serviceModel = new NtiWarningLetterModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				ClosedStatusId = NtiWarningLetterStatus.CancelWarningLetter
			};

			// act
			var actionSummary = serviceModel.ToActionSummary();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(actionSummary.Name, Is.EqualTo("NTI Warning Letter"));
				Assert.That(actionSummary.ClosedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.ClosedAt)));
				Assert.That(actionSummary.OpenedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.CreatedAt)));
				Assert.That(actionSummary.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/ntiwarningletter/{testData.Id}"));
				Assert.That(actionSummary.StatusName, Is.EqualTo("Cancelled"));
			});

			actionSummary.RawOpenedDate.Should().Be(testData.CreatedAt);
			actionSummary.RawClosedDate.Should().Be(testData.ClosedAt);
		}

		[Test]
		public void WhenMapDbModelToActionSummary_WhenActionIsClosedWithNoStatus_ReturnsEmpty()
		{
			// Validation should catch this test case, but just in case, we have to handle it
			// Closed status is optional, so the database and api will not stop someone from closing a case without a status
			// The client should catch this, but just in case there is a regression, we should handle it
			var model = _fixture.Create<NtiWarningLetterModel>();
			model.ClosedStatusId = null;

			var actionSummary = model.ToActionSummary();

			actionSummary.StatusName.Should().BeNull();
		}

		[TestCaseSource(nameof(GetStatusTestCases))]
		public void WhenMapDbModelToActionSummary_WhenActionIsOpen_ReturnsCorrectStatus(
			NtiWarningLetterStatus? status, 
			string expectedResult)
		{
			var model = _fixture.Create<NtiWarningLetterModel>();
			model.ClosedAt = null;
			model.Status = status;

			var result = model.ToActionSummary();

			result.StatusName.Should().Be(expectedResult);
		}

		private static IEnumerable<TestCaseData> GetStatusTestCases()
		{
			yield return new TestCaseData(null, "In progress");
			yield return new TestCaseData(NtiWarningLetterStatus.PreparingWarningLetter, "Preparing warning letter");
		}

		private static IEnumerable<TestCaseData> GetPermissionTestCases()
		{
			yield return new TestCaseData(new DateTime(), new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
			yield return new TestCaseData(null, new GetCasePermissionsResponse());
		}
	}
}