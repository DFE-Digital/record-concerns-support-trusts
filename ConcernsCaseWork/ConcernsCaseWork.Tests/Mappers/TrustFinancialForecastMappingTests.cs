using AutoFixture;
using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Mappers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConcernsCaseWork.Tests.Mappers;

[Parallelizable(ParallelScope.All)]
public class TrustFinancialForecastMappingTests
{
	private readonly static Fixture _fixture = new();

	[Test]
	public void WhenMaptoViewModel_IsOpen_ReturnsCorrectModel()
	{
		var response = _fixture.Create<TrustFinancialForecastResponse>();
		response.ClosedAt = null;
		response.SRMAOfferedAfterTFF = SRMAOfferedAfterTFF.Yes;
		response.ForecastingToolRanAt = ForecastingToolRanAt.CurrentYearSpring;
		response.WasTrustResponseSatisfactory = WasTrustResponseSatisfactory.Satisfactory;
		response.CreatedAt = new DateTime(2022, 01, 05);
		response.SFSOInitialReviewHappenedAt = new DateTime(2022, 03, 02);
		response.TrustRespondedAt = new DateTime(2022, 11, 12);

		var permissionsResponse = new GetCasePermissionsResponse() { Permissions = new List<CasePermission> { CasePermission.Edit } };

		var result = TrustFinancialForecastMapping.ToViewModel(response, permissionsResponse);

		result.IsClosed.Should().BeFalse();
		result.IsEditable.Should().BeTrue();
		result.SRMAOfferedAfterTFF.Should().Be("Yes");
		result.ForecastingToolRanAt.Should().Be("Current year - Spring");
		result.WasTrustResponseSatisfactory.Should().Be("Satisfactory");
		result.Notes.Should().Be(response.Notes);
		result.DateOpened.Should().Be("05 January 2022");
		result.SFSOInitialReviewHappenedAt.Should().Be("02 March 2022");
		result.TrustRespondedAt.Should().Be("12 November 2022");
		result.DateClosed.Should().BeEmpty();
	}

	[Test]
	public void WhenMapToViewModel_IsClosed_ReturnsCorrectModel()
	{
		var response = _fixture.Create<TrustFinancialForecastResponse>();
		response.ClosedAt = new DateTime(2022, 5, 7);

		var result = TrustFinancialForecastMapping.ToViewModel(response, new GetCasePermissionsResponse());

		result.IsClosed.Should().BeTrue();
		result.DateClosed.Should().Be("07 May 2022");
	}

	[TestCaseSource(nameof(PermissionTestCases))]
	public void WhenMapToViewModel_IsNotEditable_ReturnsCorrectModel(DateTime? closedAt, GetCasePermissionsResponse permissionResponse)
	{
		var tffResponse = _fixture.Create<TrustFinancialForecastResponse>();
		tffResponse.ClosedAt = closedAt;

		var result = TrustFinancialForecastMapping.ToViewModel(tffResponse, permissionResponse);

		result.IsEditable.Should().BeFalse();
	}
	
	[Test]
	public void WhenMapToActionSummary_WhenClosed_ReturnsActionSummary()
	{
		//arrange
		var testData = new
		{
			Id = _fixture.Create<int>(),
			CaseUrn = _fixture.Create<int>(),
			SRMAOfferedAfterTFF = _fixture.Create<SRMAOfferedAfterTFF>(),
			ForecastingToolRanAt = _fixture.Create<ForecastingToolRanAt>(),
			WasTrustResponseSatisfactory = _fixture.Create<WasTrustResponseSatisfactory>(),
			Notes = _fixture.Create<string>(),
			SFSOInitialReviewHappenedAt = _fixture.Create<DateTime>(),
			TrustRespondedAt = _fixture.Create<DateTime>(),
			UpdatedAt = _fixture.Create<DateTimeOffset>(),
			CreatedAt = new DateTimeOffset(2021, 9, 5, 0, 0, 0, new TimeSpan()),
			ClosedAt = new DateTimeOffset(2024, 8, 30, 0, 0, 0, new TimeSpan())
		};

		var serviceModel = new TrustFinancialForecastResponse
		{
			TrustFinancialForecastId = testData.Id,
			CaseUrn = testData.CaseUrn,
			SRMAOfferedAfterTFF = testData.SRMAOfferedAfterTFF,
			ForecastingToolRanAt = testData.ForecastingToolRanAt,
			WasTrustResponseSatisfactory = testData.WasTrustResponseSatisfactory,
			Notes = testData.Notes,
			SFSOInitialReviewHappenedAt = testData.SFSOInitialReviewHappenedAt,
			TrustRespondedAt = testData.TrustRespondedAt,
			CreatedAt = testData.CreatedAt,
			UpdatedAt = testData.UpdatedAt,
			ClosedAt = testData.ClosedAt,
		};

		// act
		var result = serviceModel.ToActionSummary();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Name, Is.EqualTo("Trust Financial Forecast (TFF)"));
			Assert.That(result.ClosedDate, Is.EqualTo("30 August 2024"));
			Assert.That(result.OpenedDate, Is.EqualTo("05 September 2021"));
			Assert.That(result.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/trustfinancialforecast/{testData.Id}"));
			Assert.That(result.StatusName, Is.EqualTo("Completed"));
		});

		result.RawOpenedDate.Should().Be(testData.CreatedAt);
		result.RawClosedDate.Should().Be(testData.ClosedAt);
	}
	
	[Test]
	public void WhenMapToActionSummary_WhenOpen_ReturnsActionSummary()
	{
		//arrange
		var testData = new
		{
			Id = _fixture.Create<int>(),
			CaseUrn = _fixture.Create<int>(),
			SRMAOfferedAfterTFF = _fixture.Create<SRMAOfferedAfterTFF>(),
			ForecastingToolRanAt = _fixture.Create<ForecastingToolRanAt>(),
			WasTrustResponseSatisfactory = _fixture.Create<WasTrustResponseSatisfactory>(),
			Notes = _fixture.Create<string>(),
			SFSOInitialReviewHappenedAt = _fixture.Create<DateTime>(),
			TrustRespondedAt = _fixture.Create<DateTime>(),
			CreatedAt = new DateTimeOffset(2021, 9, 5, 0, 0, 0, new TimeSpan()),
			UpdatedAt = _fixture.Create<DateTimeOffset>()
		};

		var serviceModel = new TrustFinancialForecastResponse
		{
			TrustFinancialForecastId = testData.Id,
			CaseUrn = testData.CaseUrn,
			SRMAOfferedAfterTFF = testData.SRMAOfferedAfterTFF,
			ForecastingToolRanAt = testData.ForecastingToolRanAt,
			WasTrustResponseSatisfactory = testData.WasTrustResponseSatisfactory,
			Notes = testData.Notes,
			SFSOInitialReviewHappenedAt = testData.SFSOInitialReviewHappenedAt,
			TrustRespondedAt = testData.TrustRespondedAt,
			CreatedAt = testData.CreatedAt,
			UpdatedAt = testData.UpdatedAt,
			ClosedAt = null
		};

		// act
		var result = serviceModel.ToActionSummary();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Name, Is.EqualTo("Trust Financial Forecast (TFF)"));
			Assert.That(result.ClosedDate, Is.EqualTo(""));
			Assert.That(result.OpenedDate, Is.EqualTo("05 September 2021"));
			Assert.That(result.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/trustfinancialforecast/{testData.Id}"));
			Assert.That(result.StatusName, Is.EqualTo("In progress"));
		});

		result.RawOpenedDate.Should().Be(testData.CreatedAt);
		result.RawClosedDate.Should().Be(null);
	}

	[Test]
	public void WhenMapToTrustFinancialForecastSummaryModel_WhenClosed_ReturnsSummaryModel()
	{
		//arrange
		var testData = new
		{
			Id = _fixture.Create<int>(),
			CaseUrn = _fixture.Create<int>(),
			CreatedAt = _fixture.Create<DateTimeOffset>(),
			UpdatedAt = _fixture.Create<DateTimeOffset>(),
			ClosedAt = _fixture.Create<DateTimeOffset?>()
		};

		Debug.Assert(testData.ClosedAt != null, "testData.ClosedAt != null");
		var serviceModel = new TrustFinancialForecastResponse
		{
			TrustFinancialForecastId = testData.Id,
			CaseUrn = testData.CaseUrn,
			CreatedAt = testData.CreatedAt.DateTime,
			UpdatedAt = testData.UpdatedAt.DateTime,
			ClosedAt = testData.ClosedAt.Value!.DateTime
		};

		// act
		var result = serviceModel.ToTrustFinancialForecastSummaryModel();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Id, Is.EqualTo(testData.Id));
			Assert.That(result.ClosedAt, Is.EqualTo(testData.ClosedAt?.DateTime));
			Assert.That(result.CreatedAt, Is.EqualTo(testData.CreatedAt.DateTime));
			Assert.That(result.UpdatedAt, Is.EqualTo(testData.UpdatedAt.DateTime));
			Assert.That(result.CaseUrn, Is.EqualTo(testData.CaseUrn));
		});
	}
	
	[Test]
	public void WhenMapToTrustFinancialForecastSummaryModel_WhenOpen_ReturnsSummaryModel()
	{
		//arrange
		var testData = new
		{
			Id = _fixture.Create<int>(),
			CaseUrn = _fixture.Create<int>(),
			CreatedAt = _fixture.Create<DateTimeOffset>(),
			UpdatedAt = _fixture.Create<DateTimeOffset>()
		};

		var serviceModel = new TrustFinancialForecastResponse
		{
			TrustFinancialForecastId = testData.Id,
			CaseUrn = testData.CaseUrn,
			CreatedAt = testData.CreatedAt.DateTime,
			UpdatedAt = testData.UpdatedAt.DateTime,
			ClosedAt = null
		};

		// act
		var result = serviceModel.ToTrustFinancialForecastSummaryModel();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Id, Is.EqualTo(testData.Id));
			Assert.That(result.CaseUrn, Is.EqualTo(testData.CaseUrn));
			Assert.That(result.ClosedAt, Is.Null);
			Assert.That(result.CreatedAt, Is.EqualTo(testData.CreatedAt.DateTime));
			Assert.That(result.UpdatedAt, Is.EqualTo(testData.UpdatedAt.DateTime));
		});
	}

	private static IEnumerable<TestCaseData> PermissionTestCases()
	{
		yield return new TestCaseData(new DateTime(), new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
		yield return new TestCaseData(null, new GetCasePermissionsResponse());
	}
}