using AutoFixture;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using FluentAssertions;
using ConcernsCaseWork.Service.NtiWarningLetter;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
using ConcernsCaseWork.Helpers;

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
				Reasons = new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(1, "Reason1") },
				SentDate = DateTime.Now.AddDays(-1),
				Status = new KeyValuePair<int, string>(1, "Status 1"),
				UpdatedAt = DateTime.Now.AddDays(-1),
				ClosedStatus = new KeyValuePair<int, string>(3, "Status 3"),
			};

			var statuses = new List<NtiWarningLetterStatusDto>();
			statuses.Add(new NtiWarningLetterStatusDto { Id = 1, Name = "Status 1" });
			statuses.Add(new NtiWarningLetterStatusDto { Id = 2, Name = "Status 2" });
			statuses.Add(new NtiWarningLetterStatusDto { Id = 3, Name = "Status 3" });

			var ntiDto = new NtiWarningLetterDto
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = null,
				Notes = testData.Notes,
				DateLetterSent = testData.SentDate,
				StatusId = testData.Status.Key,
				UpdatedAt = testData.UpdatedAt,
				WarningLetterReasonsMapping = testData.Reasons.Select(r => r.Key).ToArray(),
				ClosedStatusId = testData.ClosedStatus.Key
			};

			var permissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };

			// act
			var serviceModel = NtiWarningLetterMappers.ToServiceModel(ntiDto, statuses, permissionsResponse);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
			Assert.That(serviceModel.Id, Is.EqualTo(testData.Id));
			Assert.That(serviceModel.Reasons, Is.Not.Null);
			Assert.That(serviceModel.Reasons.Count, Is.EqualTo(testData.Reasons.Length));
			Assert.That(serviceModel.Reasons.ElementAt(0).Id, Is.EqualTo(testData.Reasons.ElementAt(0).Key));
			Assert.That(serviceModel.Status.Id, Is.EqualTo(testData.Status.Key));
			Assert.That(serviceModel.Status.Name, Is.EqualTo(testData.Status.Value));
			Assert.That(serviceModel.ClosedStatus.Id, Is.EqualTo(testData.ClosedStatus.Key));
			Assert.That(serviceModel.ClosedStatus.Name, Is.EqualTo(testData.ClosedStatus.Value));
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
			var ntiStatuses = NTIStatusFactory.BuildListNTIStatusDto();

			var serviceModel = NtiMappers.ToServiceModel(ntiDto, ntiStatuses, casePermissionsResponse);

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
				SentDate = DateTime.Now.AddDays(-1),
				Status = new KeyValuePair<int, string>(1, "Status 1"),
				UpdatedAt = DateTime.Now.AddDays(-1)
			};

			var serviceModel = new NtiWarningLetterModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				Notes = testData.Notes,
				Reasons = new NtiWarningLetterReasonModel[] { new NtiWarningLetterReasonModel { Id = testData.Reasons.First().Key, Name = testData.Reasons.First().Value } },
				Status = new NtiWarningLetterStatusModel { Id = testData.Status.Key, Name = testData.Status.Value },
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
			Assert.That(dbModel.StatusId, Is.EqualTo(testData.Status.Key));
		}

		[Test]
		public void NtiWarningLetterReason_Dto_To_ServiceModel()
		{
			// arrange
			var dto = new NtiWarningLetterReasonDto
			{
				Id = 1,
				Name = "Name Name",
				CreatedAt = new DateTime(2022, 02, 05),
				UpdatedAt = new DateTime(2022, 02, 05)
			};

			// act
			var model = NtiWarningLetterMappers.ToServiceModel(dto);

			// assert
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Id, Is.EqualTo(dto.Id));
			Assert.That(model.Name, Is.EqualTo(dto.Name));
		}

		[Test]
		public void NtiWarningLetterStatus_Dto_To_ServiceModel()
		{
			// arrange
			var dto = new NtiWarningLetterStatusDto
			{
				Id = 1,
				Name = "Name Name",
				CreatedAt = new DateTime(2022, 02, 05),
				UpdatedAt = new DateTime(2022, 02, 05)
			};

			// act
			var model = NtiWarningLetterMappers.ToServiceModel(dto);

			// assert
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Id, Is.EqualTo(dto.Id));
			Assert.That(model.Name, Is.EqualTo(dto.Name));
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
				Status = _fixture.Create<NtiWarningLetterStatusDto>()
			};

			var serviceModel = new NtiWarningLetterModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				ClosedStatus = new NtiWarningLetterStatusModel { Id = testData.Status.Id, PastTenseName = testData.Status.Name },
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
				Assert.That(actionSummary.StatusName, Is.EqualTo(testData.Status.Name));
			});

			actionSummary.RawOpenedDate.Should().Be(testData.CreatedAt);
			actionSummary.RawClosedDate.Should().Be(testData.ClosedAt);
		}

		[TestCaseSource(nameof(GetStatusTestCases))]
		public void WhenMapDbModelToActionSummary_WhenActionIsOpen_ReturnsCorrectStatus(
			NtiWarningLetterStatusModel? status, 
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
			yield return new TestCaseData(new NtiWarningLetterStatusModel() { Name = "Test"}, "Test");
		}

		private static IEnumerable<TestCaseData> GetPermissionTestCases()
		{
			yield return new TestCaseData(new DateTime(), new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
			yield return new TestCaseData(null, new GetCasePermissionsResponse());
		}
	}
}