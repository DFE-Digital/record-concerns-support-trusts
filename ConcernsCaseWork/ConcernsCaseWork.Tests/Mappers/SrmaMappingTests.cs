using AutoFixture;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.CaseActions;
using NUnit.Framework;
using System;

namespace ConcernsCaseWork.Tests.Mappers;

[Parallelizable(ParallelScope.All)]
public class SrmaMappingTests
{
	private readonly static Fixture _fixture = new();

	[Test]
	public void WhenMapDtoToServiceModel_ReturnsCorrectModel()
	{
		//arrange
		var dto = _fixture.Create<SRMADto>();

		// act
		var serviceModel = CaseActionsMapping.Map(dto);

		// assert
		Assert.That(serviceModel, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(serviceModel.CaseUrn, Is.EqualTo(dto.CaseUrn));
			Assert.That(serviceModel.Id, Is.EqualTo(dto.Id));
			Assert.That(serviceModel.Status.ToString(), Is.EqualTo(dto.Status.ToString()));
			Assert.That(serviceModel.Notes, Is.EqualTo(dto.Notes));
			Assert.That(serviceModel.Reason.ToString(), Is.EqualTo(dto.Reason!.Value.ToString()));
			Assert.That(serviceModel.DateAccepted, Is.EqualTo(dto.DateAccepted));
			Assert.That(serviceModel.DateOffered, Is.EqualTo(dto.DateOffered));
			Assert.That(serviceModel.DateVisitEnd, Is.EqualTo(dto.DateVisitEnd));
			Assert.That(serviceModel.DateVisitStart, Is.EqualTo(dto.DateVisitStart));
			Assert.That(serviceModel.ClosedAt, Is.EqualTo(dto.ClosedAt));
			Assert.That(serviceModel.CreatedAt, Is.EqualTo(dto.CreatedAt));
			Assert.That(serviceModel.UpdatedAt, Is.EqualTo(DateTime.MinValue)); // Not sure if this is correct behaviour??
		});
	}

	[Test]
	public void WhenMapModelToDto_ReturnsCorrectDto()
	{
		//arrange
		var model = _fixture.Create<SRMAModel>();

		// act
		var dto = CaseActionsMapping.Map(model);

		// assert
		Assert.That(dto, Is.Not.Null);
		Assert.Multiple(() =>
		{
			Assert.That(dto.CaseUrn, Is.EqualTo(model.CaseUrn));
			Assert.That(dto.Id, Is.EqualTo(model.Id));
			Assert.That(dto.Status.ToString(), Is.EqualTo(model.Status.ToString()));
			Assert.That(dto.Notes, Is.EqualTo(model.Notes));
			Assert.That(dto.Reason!.Value.ToString(), Is.EqualTo(model.Reason.ToString()));
			Assert.That(dto.DateAccepted, Is.EqualTo(model.DateAccepted));
			Assert.That(dto.DateOffered, Is.EqualTo(model.DateOffered));
			Assert.That(dto.DateVisitEnd, Is.EqualTo(model.DateVisitEnd));
			Assert.That(dto.DateVisitStart, Is.EqualTo(model.DateVisitStart));
			Assert.That(dto.ClosedAt, Is.EqualTo(model.ClosedAt));
			Assert.That(dto.CreatedAt, Is.EqualTo(model.CreatedAt));
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
			DateOffered = _fixture.Create<DateTime>(),
			DateAccepted = _fixture.Create<DateTime>(),
			DateReportSentToTrust = _fixture.Create<DateTime>(),
			DateVisitStart = _fixture.Create<DateTime>(),
			DateVisitEnd = _fixture.Create<DateTime>(),
			CreatedBy = _fixture.Create<string>(),
			Notes = _fixture.Create<string>(),
			Status = _fixture.Create<Enums.SRMAStatus>(),
			SRMAReasonOffered = _fixture.Create<Enums.SRMAReasonOffered>(),
			CreatedAt = _fixture.Create<DateTime>(),
			UpdatedAt = _fixture.Create<DateTime>(),
			ClosedAt = _fixture.Create<DateTime>()
		};

		var serviceModel = new SRMAModel(
			testData.Id, 
			testData.CaseUrn, 
			testData.DateOffered, 
			testData.DateAccepted, 
			testData.DateReportSentToTrust, 
			testData.DateVisitStart, 
			testData.DateVisitEnd, 
			testData.Status, 
			testData.Notes, 
			testData.SRMAReasonOffered, 
			testData.CreatedAt)
		{
			UpdatedAt = testData.UpdatedAt, 
			ClosedAt = testData.ClosedAt,
			Status = testData.Status
		};

		// act
		var actionSummary = serviceModel.ToActionSummary();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(actionSummary.Name, Is.EqualTo("SRMA"));
			Assert.That(actionSummary.ClosedDate, Is.EqualTo(testData.ClosedAt.GetFormattedDate()));
			Assert.That(actionSummary.OpenedDate, Is.EqualTo(testData.CreatedAt.GetFormattedDate()));
			Assert.That(actionSummary.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/srma/{testData.Id}/closed"));
			Assert.That(actionSummary.StatusName, Is.EqualTo(EnumHelper.GetEnumDescription(testData.Status)));
		});
	}
}