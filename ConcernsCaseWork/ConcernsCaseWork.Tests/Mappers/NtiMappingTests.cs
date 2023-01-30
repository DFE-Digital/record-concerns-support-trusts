using AutoFixture;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Shared.Tests.Factory;
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
				Reasons = new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(1, "Reason1") },
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
				StatusId = testData.Status.Key,
				UpdatedAt = testData.UpdatedAt,
				ReasonsMapping = testData.Reasons.Select(r => r.Key).ToArray(),
				SumissionDecisionId = testData.SumissionDecisionId,
				DateNTIClosed = testData.DateNTIClosed,
				DateNTILifted = testData.DateNTILifted
			};

			var casePermissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };

			var ntiStatuses = NTIStatusFactory.BuildListNTIStatusDto();
			// act
			var serviceModel = NtiMappers.ToServiceModel(ntiDto, ntiStatuses, casePermissionsResponse);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
			Assert.That(serviceModel.Id, Is.EqualTo(testData.Id));
			Assert.That(serviceModel.Reasons, Is.Not.Null);
			Assert.That(serviceModel.Reasons.Count, Is.EqualTo(testData.Reasons.Length));
			Assert.That(serviceModel.Reasons.ElementAt(0).Id, Is.EqualTo(testData.Reasons.ElementAt(0).Key));
			Assert.That(serviceModel.Status, Is.Not.Null);
			Assert.That(serviceModel.SumissionDecisionId, Is.EqualTo(testData.SumissionDecisionId));
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
				DateStarted = DateTime.Now.AddDays(-1),
				Status = new KeyValuePair<int, string>(1, "Status 1"),
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
				Reasons = new NtiReasonModel[] { new NtiReasonModel { Id = testData.Reasons.First().Key, Name = testData.Reasons.First().Value } },
				Status = new NtiStatusModel { Id = testData.Status.Key, Name = testData.Status.Value },
				DateStarted = testData.DateStarted,
				UpdatedAt = testData.UpdatedAt,
				SumissionDecisionId = testData.SumissionDecisionId,
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
			Assert.That(dbModel.StatusId, Is.EqualTo(testData.Status.Key));
			Assert.That(dbModel.Notes, Is.EqualTo(testData.Notes));
			Assert.That(dbModel.ReasonsMapping.First(), Is.EqualTo(testData.Reasons.First().Key));
			Assert.That(dbModel.DateStarted, Is.EqualTo(testData.DateStarted));
			Assert.That(dbModel.UpdatedAt, Is.EqualTo(testData.UpdatedAt));
			Assert.That(serviceModel.SumissionDecisionId, Is.EqualTo(testData.SumissionDecisionId));
			Assert.That(serviceModel.DateNTILifted, Is.EqualTo(testData.DateNTILifted));
			Assert.That(serviceModel.DateNTIClosed, Is.EqualTo(testData.DateNTIClosed));
		}

		[Test]
		public void NtiReason_Dto_To_ServiceModel()
		{
			// arrange
			var dto = new NtiReasonDto
			{
				Id = 1,
				Name = "Name Name"
			};

			// act
			var model = NtiMappers.ToServiceModel(dto);

			// assert
			Assert.That(model, Is.Not.Null);
			Assert.That(model.Id, Is.EqualTo(dto.Id));
			Assert.That(model.Name, Is.EqualTo(dto.Name));
		}

		[Test]
		public void NtiStatus_Dto_To_ServiceModel()
		{
			// arrange
			var dto = new NtiStatusDto
			{
				Id = 1,
				Name = "Name Name"
			};

			// act
			var model = NtiMappers.ToServiceModel(dto);

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
				ClosedStatus = new KeyValuePair<int, string>(_fixture.Create<int>(), _fixture.Create<string>())
			};

			var serviceModel = new NtiModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				ClosedStatus = new NtiStatusModel { Id = testData.ClosedStatus.Key, Name = testData.ClosedStatus.Value },
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
				Assert.That(actionSummary.StatusName, Is.EqualTo(testData.ClosedStatus.Value));
			});

			actionSummary.RawOpenedDate.Should().Be(testData.CreatedAt);
			actionSummary.RawClosedDate.Should().Be(testData.ClosedAt);
		}

		[TestCaseSource(nameof(GetStatusTestCases))]
		public void WhenMapDbModelToActionSummary_WhenActionIsOpen_ReturnsCorrectStatus(
			NtiStatusModel? status,
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
			 yield return new TestCaseData(new NtiStatusModel() { Name = "Test" }, "Test");
		}

		private static IEnumerable<TestCaseData> GetPermissionTestCases()
		{
			yield return new TestCaseData(new DateTime(), new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
			yield return new TestCaseData(null, new GetCasePermissionsResponse());
		}
	}
}