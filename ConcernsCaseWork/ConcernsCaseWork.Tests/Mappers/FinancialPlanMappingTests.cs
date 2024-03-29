using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ConcernsCaseWork.Service.FinancialPlan;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.API.Contracts.FinancialPlan;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class FinancialPlanMappingTests
	{
		private readonly static Fixture _fixture = new();

		[Test]
		public void WhenMapDtoToServiceModel_ReturnsCorrectModel()
		{
			//arrange
			var testData = new
			{
				Id = _fixture.Create<long>(),
				CaseUrn = _fixture.Create<long>(),
				CreatedAt = _fixture.Create<DateTime>(),
				ClosedAt = _fixture.Create<DateTime>(),
				UpdatedAt = _fixture.Create<DateTime>(),
				CreatedBy = _fixture.Create<string>(),
				Notes = _fixture.Create<string>(),
				DatePlanRequested = _fixture.Create<DateTime>(),
				Status = FinancialPlanStatus.AwaitingPlan,
				DatePlanReceived = _fixture.Create<DateTime>(),
				ClosedStatus = new KeyValuePair<int, string>(_fixture.Create<int>(), _fixture.Create<string>()),
			};

			var dto = new FinancialPlanDto
			(
				testData.Id,
				testData.CaseUrn,
				testData.CreatedAt,
				testData.ClosedAt,
				testData.CreatedBy,
				1,
				testData.DatePlanRequested,
				testData.DatePlanReceived,
				testData.Notes,
				testData.UpdatedAt
			);

			var casePermission = new GetCasePermissionsResponse() { Permissions  = new List<CasePermission>() { CasePermission.Edit } };

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dto, casePermission);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(serviceModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
				Assert.That(serviceModel.Id, Is.EqualTo(testData.Id));
				Assert.That(serviceModel.Status, Is.EqualTo(testData.Status));
				Assert.That(serviceModel.Notes, Is.EqualTo(testData.Notes));
				Assert.That(serviceModel.DatePlanRequested, Is.EqualTo(testData.DatePlanRequested));
				Assert.That(serviceModel.DateViablePlanReceived, Is.EqualTo(testData.DatePlanReceived));
				Assert.That(serviceModel.UpdatedAt, Is.EqualTo(testData.UpdatedAt));
				Assert.That(serviceModel.ClosedAt, Is.EqualTo(testData.ClosedAt));
				Assert.That(serviceModel.CreatedAt, Is.EqualTo(testData.CreatedAt));
				Assert.That(serviceModel.IsEditable, Is.True);
			});
		}

		[Test]
		public void WhenMapDtoToServiceModel_NotEditable_ReturnsCorrectModel()
		{
			var dto = _fixture.Create<FinancialPlanDto>();
			dto.StatusId = (int)FinancialPlanStatus.AwaitingPlan;

			var casePermission = new GetCasePermissionsResponse();

			var serviceModel = FinancialPlanMapping.MapDtoToModel(dto, casePermission);

			serviceModel.IsEditable.Should().BeFalse();
		}

		[Test]
		public void WhenMapDtoListToDbModelList_ReturnsCorrectModelList()
		{
			//arrange
			var testData = new
			{
				Id = _fixture.Create<long>(),
				CaseUrn = _fixture.Create<long>(),
				CreatedAt = _fixture.Create<DateTime>(),
				ClosedAt = _fixture.Create<DateTime>(),
				UpdatedAt = _fixture.Create<DateTime>(),
				CreatedBy = _fixture.Create<string>(),
				Notes = _fixture.Create<string>(),
				DatePlanRequested = _fixture.Create<DateTime>(),
				Status = FinancialPlanStatus.AwaitingPlan,
				DatePlanReceived = _fixture.Create<DateTime>(),
				ClosedStatus = new KeyValuePair<int, string>(_fixture.Create<int>(), _fixture.Create<string>()),
			};

			var dtos = new List<FinancialPlanDto>
			{
				new
				(
					testData.Id,
					testData.CaseUrn,
					testData.CreatedAt,
					testData.ClosedAt,
					testData.CreatedBy,
					1,
					testData.DatePlanRequested,
					testData.DatePlanReceived,
					testData.Notes,
					testData.UpdatedAt
				)
			};

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dtos);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(serviceModel.Count, Is.EqualTo(1));
				Assert.That(serviceModel.Single().CaseUrn, Is.EqualTo(testData.CaseUrn));
				Assert.That(serviceModel.Single().Id, Is.EqualTo(testData.Id));
				Assert.That(serviceModel.Single().Status, Is.EqualTo(testData.Status));
				Assert.That(serviceModel.Single().Notes, Is.EqualTo(testData.Notes));
				Assert.That(serviceModel.Single().DatePlanRequested, Is.EqualTo(testData.DatePlanRequested));
				Assert.That(serviceModel.Single().DateViablePlanReceived, Is.EqualTo(testData.DatePlanReceived));
				Assert.That(serviceModel.Single().UpdatedAt, Is.EqualTo(testData.UpdatedAt));
				Assert.That(serviceModel.Single().ClosedAt, Is.EqualTo(testData.ClosedAt));
				Assert.That(serviceModel.Single().CreatedAt, Is.EqualTo(testData.CreatedAt));
			});
		}

		[Test]
		public void WhenMapNullDtoListToDbModelList_ReturnsEmptyModelList()
		{
			//arrange
			List<FinancialPlanDto> dtos = null;

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dtos);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel, Is.Empty);
		}

		[Test]
		public void WhenMapEmptyDtoListToDbModelList_ReturnsEmptyModelList()
		{
			//arrange
			var dtos = new List<FinancialPlanDto>();

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dtos);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel, Is.Empty);
		}

		[Test]
		public void WhenMapPatchFinancialPlanModelToDto_ReturnsCorrectModel()
		{
			//arrange
			var testData = new
			{
				Id = _fixture.Create<long>(),
				CaseUrn = _fixture.Create<long>(),
				CreatedAt = _fixture.Create<DateTime>(),
				ClosedAt = _fixture.Create<DateTime>(),
				UpdatedAt = _fixture.Create<DateTime>(),
				CreatedBy = _fixture.Create<string>(),
				Notes = _fixture.Create<string>(),
				DatePlanRequested = _fixture.Create<DateTime>(),
				Status = FinancialPlanStatus.AwaitingPlan,
				DatePlanReceived = _fixture.Create<DateTime>(),
				ClosedStatus = new KeyValuePair<int, string>(_fixture.Create<int>(), _fixture.Create<string>()),
			};

			var dto = new FinancialPlanDto
			(
				testData.Id,
				testData.CaseUrn,
				testData.CreatedAt,
				_fixture.Create<DateTime>(),
				testData.CreatedBy,
				_fixture.Create<int>(),
				_fixture.Create<DateTime>(),
				_fixture.Create<DateTime>(),
				_fixture.Create<string>(),
				_fixture.Create<DateTime>()
			);

			var model = new PatchFinancialPlanModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				ClosedAt = testData.ClosedAt,
				StatusId = (long)testData.Status,
				DatePlanRequested = testData.DatePlanRequested,
				DateViablePlanReceived = testData.DatePlanReceived,
				Notes = testData.Notes
			};

			// act
			var result = FinancialPlanMapping.MapPatchFinancialPlanModelToDto(model, dto);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(result.CaseUrn, Is.EqualTo(testData.CaseUrn));
				Assert.That(result.Id, Is.EqualTo(testData.Id));
				Assert.That(result.StatusId, Is.EqualTo((long)testData.Status));
				Assert.That(result.Notes, Is.EqualTo(testData.Notes));
				Assert.That(result.DatePlanRequested, Is.EqualTo(testData.DatePlanRequested));
				Assert.That(result.DateViablePlanReceived, Is.EqualTo(testData.DatePlanReceived));
				Assert.That(result.ClosedAt, Is.EqualTo(testData.ClosedAt));
				Assert.That(result.CreatedAt, Is.EqualTo(testData.CreatedAt));
				Assert.That(result.CreatedBy, Is.EqualTo(testData.CreatedBy));
			});
		}

		[Test]
		public void WhenMapDbModelToActionSummary_ReturnsCorrectModel()
		{
			//arrange
			var testData = new
			{
				Id = _fixture.Create<long>(),
				CaseUrn = _fixture.Create<long>(),
				CreatedAt = _fixture.Create<DateTime>(),
				ClosedAt = _fixture.Create<DateTime>(),
				UpdatedAt = _fixture.Create<DateTime>(),
				CreatedBy = _fixture.Create<string>(),
				Notes = _fixture.Create<string>(),
				DatePlanRequested = _fixture.Create<DateTime>(),
				Status = FinancialPlanStatus.AwaitingPlan,
				DatePlanReceived = _fixture.Create<DateTime>()
			};

			var serviceModel = new FinancialPlanModel(
				testData.Id,
				testData.CaseUrn,
				testData.CreatedAt,
				testData.DatePlanRequested,
				testData.DatePlanReceived,
				testData.Notes,
				testData.Status,
				testData.ClosedAt,
				testData.UpdatedAt
			)
			{ UpdatedAt = testData.UpdatedAt };

			// act
			var actionSummary = serviceModel.ToActionSummary();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(actionSummary.Name, Is.EqualTo("Financial Plan"));
				Assert.That(actionSummary.ClosedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.ClosedAt)));
				Assert.That(actionSummary.OpenedDate, Is.EqualTo(DateTimeHelper.ParseToDisplayDate(testData.CreatedAt)));
				Assert.That(actionSummary.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/financialplan/{testData.Id}/closed"));
				Assert.That(actionSummary.StatusName, Is.EqualTo("Awaiting plan"));
			});

			actionSummary.RawOpenedDate.Should().Be(testData.CreatedAt);
			actionSummary.RawClosedDate.Should().Be(testData.ClosedAt);
		}

		[Test]
		public void WhenMapDbModelToActionSummary_WhenActionSummaryIsOpen_ReturnsCorrectModel()
		{
			var financialPlan = _fixture.Create<FinancialPlanModel>();
			financialPlan.ClosedAt = null;
			financialPlan.Status = null;

			var result = financialPlan.ToActionSummary();
			result.RelativeUrl.Should().Be($"/case/{financialPlan.CaseUrn}/management/action/financialplan/{financialPlan.Id}");
			result.StatusName.Should().Be("In progress");
			result.ClosedDate.Should().BeEmpty();
		}
	}
}