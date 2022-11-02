using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using NUnit.Framework;
using Service.TRAMS.FinancialPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework.Interfaces;

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
			var statuses = _fixture.CreateMany<FinancialPlanStatusDto>().ToList();

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
				Status = statuses.First(),
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
				testData.Status.Id,
				testData.DatePlanRequested,
				testData.DatePlanReceived,
				testData.Notes
			);

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dto, statuses);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(serviceModel.CaseUrn, Is.EqualTo(testData.CaseUrn));
				Assert.That(serviceModel.Id, Is.EqualTo(testData.Id));
				Assert.That(serviceModel.Status.Id, Is.EqualTo(testData.Status.Id));
				Assert.That(serviceModel.Status.Name, Is.EqualTo(testData.Status.Name));
				Assert.That(serviceModel.Notes, Is.EqualTo(testData.Notes));
				Assert.That(serviceModel.DatePlanRequested, Is.EqualTo(testData.DatePlanRequested));
				Assert.That(serviceModel.DateViablePlanReceived, Is.EqualTo(testData.DatePlanReceived));
				//Assert.That(serviceModel.UpdatedAt, Is.EqualTo(testData.UpdatedAt)); // TODO: This is not currently mapped. Check if this is correct behaviour
				Assert.That(serviceModel.ClosedAt, Is.EqualTo(testData.ClosedAt));
				Assert.That(serviceModel.CreatedAt, Is.EqualTo(testData.CreatedAt));
			});
		}

		[Test]
		public void WhenMapDtoListToDbModelList_ReturnsCorrectModelList()
		{
			//arrange
			var statuses = _fixture.CreateMany<FinancialPlanStatusDto>().ToList();

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
				Status = statuses.First(),
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
					testData.Status.Id,
					testData.DatePlanRequested,
					testData.DatePlanReceived,
					testData.Notes
				)
			};

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dtos, statuses);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(serviceModel.Count, Is.EqualTo(1));
				Assert.That(serviceModel.Single().CaseUrn, Is.EqualTo(testData.CaseUrn));
				Assert.That(serviceModel.Single().Id, Is.EqualTo(testData.Id));
				Assert.That(serviceModel.Single().Status.Id, Is.EqualTo(testData.Status.Id));
				Assert.That(serviceModel.Single().Status.Name, Is.EqualTo(testData.Status.Name));
				Assert.That(serviceModel.Single().Notes, Is.EqualTo(testData.Notes));
				Assert.That(serviceModel.Single().DatePlanRequested, Is.EqualTo(testData.DatePlanRequested));
				Assert.That(serviceModel.Single().DateViablePlanReceived, Is.EqualTo(testData.DatePlanReceived));
				//Assert.That(serviceModel.Single().UpdatedAt, Is.EqualTo(testData.UpdatedAt)); // TODO: This is not currently mapped. Check if this is correct behaviour
				Assert.That(serviceModel.Single().ClosedAt, Is.EqualTo(testData.ClosedAt));
				Assert.That(serviceModel.Single().CreatedAt, Is.EqualTo(testData.CreatedAt));
			});
		}

		[Test]
		public void WhenMapNullDtoListToDbModelList_ReturnsEmptyModelList()
		{
			//arrange
			var statuses = _fixture.CreateMany<FinancialPlanStatusDto>().ToList();

			List<FinancialPlanDto> dtos = null;

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dtos, statuses);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel, Is.Empty);
		}

		[Test]
		public void WhenMapEmptyDtoListToDbModelList_ReturnsEmptyModelList()
		{
			//arrange
			var statuses = _fixture.CreateMany<FinancialPlanStatusDto>().ToList();

			var dtos = new List<FinancialPlanDto>();

			// act
			var serviceModel = FinancialPlanMapping.MapDtoToModel(dtos, statuses);

			// assert
			Assert.That(serviceModel, Is.Not.Null);
			Assert.That(serviceModel, Is.Empty);
		}

		[Test]
		public void WhenMapPatchFinancialPlanModelToDto_ReturnsCorrectModel()
		{
			//arrange
			var statuses = _fixture.CreateMany<FinancialPlanStatusDto>().ToList();

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
				Status = statuses.First(),
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
				_fixture.Create<string>()
			);

			var model = new PatchFinancialPlanModel
			{
				Id = testData.Id,
				CaseUrn = testData.CaseUrn,
				ClosedAt = testData.ClosedAt,
				StatusId = testData.Status.Id,
				DatePlanRequested = testData.DatePlanRequested,
				DateViablePlanReceived = testData.DatePlanReceived,
				Notes = testData.Notes
			};

			// act
			var result = FinancialPlanMapping.MapPatchFinancialPlanModelToDto(model, dto, statuses);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(result.CaseUrn, Is.EqualTo(testData.CaseUrn));
				Assert.That(result.Id, Is.EqualTo(testData.Id));
				Assert.That(result.StatusId, Is.EqualTo(testData.Status.Id));
				Assert.That(result.Notes, Is.EqualTo(testData.Notes));
				Assert.That(result.DatePlanRequested, Is.EqualTo(testData.DatePlanRequested));
				Assert.That(result.DateViablePlanReceived, Is.EqualTo(testData.DatePlanReceived));
				//Assert.That(result.UpdatedAt, Is.EqualTo(testData.UpdatedAt)); // TODO: This is not currently mapped. Check if this is correct behaviour
				Assert.That(result.ClosedAt, Is.EqualTo(testData.ClosedAt));
				Assert.That(result.CreatedAt, Is.EqualTo(testData.CreatedAt));
				Assert.That(result.CreatedBy, Is.EqualTo(testData.CreatedBy));
			});
		}

		[Test]
		public void WhenMapPatchFinancialPlanModelToDto_WithNullValues_ReturnsCorrectModelWithExistingValues()
		{
			//arrange
			var statuses = _fixture.CreateMany<FinancialPlanStatusDto>().ToList();

			var originalData = new
			{
				Id = _fixture.Create<long>(),
				CaseUrn = _fixture.Create<long>(),
				CreatedAt = _fixture.Create<DateTime>(),
				ClosedAt = _fixture.Create<DateTime>(),
				UpdatedAt = _fixture.Create<DateTime>(),
				CreatedBy = _fixture.Create<string>(),
				Notes = _fixture.Create<string>(),
				DatePlanRequested = _fixture.Create<DateTime>(),
				Status = statuses.First(),
				DatePlanReceived = _fixture.Create<DateTime>(),
				ClosedStatus = new KeyValuePair<int, string>(_fixture.Create<int>(), _fixture.Create<string>()),
			};

			var originalDto = new FinancialPlanDto
			(
				originalData.Id,
				originalData.CaseUrn,
				originalData.CreatedAt,
				originalData.ClosedAt,
				originalData.CreatedBy,
				originalData.Status.Id,
				originalData.DatePlanRequested,
				originalData.DatePlanReceived,
				originalData.Notes
			);

			var patchModel = new PatchFinancialPlanModel
			{
				Id = originalData.Id,
				CaseUrn = originalData.CaseUrn,
				ClosedAt = null,
				StatusId = null,
				DatePlanRequested = null,
				DateViablePlanReceived = null,
				Notes = null
			};

			// act
			var result = FinancialPlanMapping.MapPatchFinancialPlanModelToDto(patchModel, originalDto, statuses);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(result.CaseUrn, Is.EqualTo(originalData.CaseUrn));
				Assert.That(result.Id, Is.EqualTo(originalData.Id));
				Assert.That(result.StatusId, Is.EqualTo(originalData.Status.Id));
				Assert.That(result.Notes, Is.EqualTo(originalData.Notes));
				Assert.That(result.DatePlanRequested, Is.EqualTo(originalData.DatePlanRequested));
				Assert.That(result.DateViablePlanReceived, Is.EqualTo(originalData.DatePlanReceived));
				Assert.That(result.ClosedAt, Is.EqualTo(originalData.ClosedAt));
				Assert.That(result.CreatedAt, Is.EqualTo(originalData.CreatedAt));
				Assert.That(result.CreatedBy, Is.EqualTo(originalData.CreatedBy));
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
				Status = new FinancialPlanStatusModel("AwaitingPlan", 1, true),
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
				testData.ClosedAt
			)
			{ UpdatedAt = testData.UpdatedAt };

			// act
			var actionSummary = serviceModel.ToActionSummary();

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(actionSummary.Name, Is.EqualTo("Financial Plan"));
				Assert.That(actionSummary.ClosedDate, Is.EqualTo(testData.ClosedAt.GetFormattedDate()));
				Assert.That(actionSummary.OpenedDate, Is.EqualTo(testData.CreatedAt.GetFormattedDate()));
				Assert.That(actionSummary.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/financialplan/{testData.Id}/closed"));
				Assert.That(actionSummary.StatusName, Is.EqualTo("Awaiting plan"));
			});
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
			result.ClosedDate.Should().BeNull();
		}
	}
}