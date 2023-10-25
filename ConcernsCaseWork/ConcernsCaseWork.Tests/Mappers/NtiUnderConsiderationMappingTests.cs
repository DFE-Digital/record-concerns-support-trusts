using AutoFixture;
using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class NtiUnderConsiderationMappingTests
	{
		private readonly Fixture _fixture = new();

		[Test]
		public void WhenMapDtoToServiceModel_ReturnsCorrectModel()
		{
			//arrange
			var ntiDto = new NtiUnderConsiderationDto
			{
				Id = 1L,
				CaseUrn = 123L,
				CreatedAt = DateTime.Now.AddDays(-5),
				ClosedAt = null,
				ClosedStatusId = 1,
				Notes = "Test notes",
				UnderConsiderationReasonsMapping = new int[] { 11, 21 },
				UpdatedAt = DateTime.Now.AddDays(-5)
			};

			var permissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } };

			// act
			var serviceModel = NtiUnderConsiderationMappers.ToServiceModel(ntiDto, permissionsResponse);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel.CaseUrn, Is.EqualTo(ntiDto.CaseUrn));
			Assert.That(serviceModel.Id, Is.EqualTo(ntiDto.Id));
			Assert.That(serviceModel.ClosedStatusId, Is.EqualTo((NtiUnderConsiderationClosedStatus)ntiDto.ClosedStatusId));
			Assert.That(serviceModel.ClosedStatusName, Is.Null);
			Assert.That(serviceModel.CreatedAt, Is.EqualTo(ntiDto.CreatedAt));
			Assert.That(serviceModel.Notes, Is.EqualTo(ntiDto.Notes));
			Assert.That(serviceModel.NtiReasonsForConsidering, Is.Not.Null);
			Assert.That(serviceModel.UpdatedAt, Is.EqualTo(ntiDto.UpdatedAt));
			Assert.That(serviceModel.NtiReasonsForConsidering.Count, Is.EqualTo(ntiDto.UnderConsiderationReasonsMapping.Count));
			Assert.That(serviceModel.NtiReasonsForConsidering.ElementAt(0), Is.EqualTo((NtiUnderConsiderationReason)ntiDto.UnderConsiderationReasonsMapping.ElementAt(0)));
			Assert.That(serviceModel.NtiReasonsForConsidering.ElementAt(1), Is.EqualTo((NtiUnderConsiderationReason)ntiDto.UnderConsiderationReasonsMapping.ElementAt(1)));
			serviceModel.IsEditable.Should().BeTrue();
		}

		[TestCaseSource(nameof(GetPermissionTestCases))]
		public void WhenMapDtoToServiceModel_Not_Editable_ReturnsCorrectModel(
			DateTime closedDate,
			GetCasePermissionsResponse casePermissionsResponse)
		{
			var ntiDto = _fixture.Create<NtiUnderConsiderationDto>();
			ntiDto.ClosedStatusId = (int)NtiUnderConsiderationClosedStatus.NoFurtherAction;
			ntiDto.ClosedAt = closedDate;

			var serviceModel = NtiUnderConsiderationMappers.ToServiceModel(ntiDto, casePermissionsResponse);

			serviceModel.IsEditable.Should().BeFalse();
			serviceModel.ClosedAt.Should().Be(closedDate);
			serviceModel.ClosedStatusName = "No further action being taken";
		}

		[Test]
		public void WhenMapDtoToDbModel_ReturnsCorrectModel()
		{
			//arrange
			var serviceModel = new NtiUnderConsiderationModel
			{
				Id = 1L,
				CaseUrn = 123L,
				CreatedAt = DateTime.Now.AddDays(-5),
				ClosedAt = DateTime.Now,
				ClosedStatusId = NtiUnderConsiderationClosedStatus.NoFurtherAction,
				ClosedStatusName = "SomeStatusName",
				Notes = "Test notes",
				NtiReasonsForConsidering = new NtiUnderConsiderationReason[] {
					NtiUnderConsiderationReason.CashFlowProblems,
					NtiUnderConsiderationReason.CumulativeDeficitActual
				}
			};

			// act
			var dbModel = NtiUnderConsiderationMappers.ToDBModel(serviceModel);

			// assert
			Assert.That(dbModel, Is.Not.Null);
			Assert.That(dbModel.CaseUrn, Is.EqualTo(serviceModel.CaseUrn));
			Assert.That(dbModel.Id, Is.EqualTo(serviceModel.Id));
			Assert.That(dbModel.CreatedAt, Is.EqualTo(serviceModel.CreatedAt));
			Assert.That(dbModel.ClosedAt, Is.EqualTo(serviceModel.ClosedAt));
			Assert.That(dbModel.ClosedStatusId, Is.EqualTo((int)serviceModel.ClosedStatusId));
			Assert.That(dbModel.Notes, Is.EqualTo(serviceModel.Notes));

			Assert.That(dbModel.UnderConsiderationReasonsMapping, Is.Not.Null);
			Assert.That(dbModel.UnderConsiderationReasonsMapping.Count, Is.EqualTo(serviceModel.NtiReasonsForConsidering.Count));
			Assert.That(dbModel.UnderConsiderationReasonsMapping.ElementAt(0), Is.EqualTo((int)serviceModel.NtiReasonsForConsidering.ElementAt(0)));
			Assert.That(dbModel.UnderConsiderationReasonsMapping.ElementAt(1), Is.EqualTo((int)serviceModel.NtiReasonsForConsidering.ElementAt(1)));

			Assert.That(dbModel.UnderConsiderationReasonsMapping, Is.Not.Null);
			Assert.That(dbModel.UnderConsiderationReasonsMapping.Count, Is.EqualTo(serviceModel.NtiReasonsForConsidering.Count));
			Assert.That(dbModel.UnderConsiderationReasonsMapping.ElementAt(0), Is.EqualTo((int)serviceModel.NtiReasonsForConsidering.ElementAt(0)));
			Assert.That(dbModel.UnderConsiderationReasonsMapping.ElementAt(1), Is.EqualTo((int)serviceModel.NtiReasonsForConsidering.ElementAt(1)));
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
				ClosedStatus = NtiUnderConsiderationClosedStatus.ToBeEscalated
			};

			var serviceModel = new NtiUnderConsiderationModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				CreatedAt = testData.CreatedAt,
				ClosedAt = testData.ClosedAt,
				ClosedStatusId = testData.ClosedStatus,
			};

			// act
			var actionSummary = serviceModel.ToActionSummary();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(actionSummary.Name, Is.EqualTo("NTI Under Consideration"));
				Assert.That(actionSummary.ClosedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.ClosedAt)));
				Assert.That(actionSummary.OpenedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.CreatedAt)));
				Assert.That(actionSummary.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/ntiunderconsideration/{testData.Id}"));
				Assert.That(actionSummary.StatusName, Is.EqualTo("To be escalated"));
			});

			actionSummary.RawOpenedDate.Should().Be(testData.CreatedAt);
			actionSummary.RawClosedDate.Should().Be(testData.ClosedAt);
		}

		[TestCaseSource(nameof(GetStatusTestCases))]
		public void WhenMapDbModelToActionSummary_ReturnsCorrectActionStatus(
			DateTime? closedAt,
			NtiUnderConsiderationClosedStatus? closedStatus,
			string expectedResult)
		{
			var model = _fixture.Create<NtiUnderConsiderationModel>();
			model.ClosedStatusId = closedStatus;
			model.ClosedAt = closedAt;

			var actualResult = model.ToActionSummary();

			actualResult.StatusName.Should().Be(expectedResult);
		}

		private static IEnumerable<TestCaseData> GetStatusTestCases()
		{
			yield return new TestCaseData(new DateTime(), null, null);
			yield return new TestCaseData(new DateTime(), NtiUnderConsiderationClosedStatus.NoFurtherAction, "No further action being taken");
			yield return new TestCaseData(null, null, "In progress");
		}

		private static IEnumerable<TestCaseData> GetPermissionTestCases()
		{
			yield return new TestCaseData(new DateTime(), new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
			yield return new TestCaseData(null, new GetCasePermissionsResponse());
		}
	}
}