using AutoFixture;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Contracts.TargetedTrustEngagement;
using ConcernsCaseWork.Mappers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Mappers;

[Parallelizable(ParallelScope.All)]
public class TargetedTrustEngagementTests
{
	private readonly static Fixture _fixture = new();

	[Test]
	public void WhenMapToActionSummary_WhenOpen_ReturnsActionSummary()
	{
		//arrange
		var testData = new
		{
			Id = _fixture.Create<int>(),
			CaseUrn = _fixture.Create<int>(),
			Title = _fixture.Create<string>(),
			CreatedAt = new DateTimeOffset(2021, 9, 5, 0, 0, 0, new TimeSpan()),
			UpdatedAt = _fixture.Create<DateTimeOffset>()
		};

		var serviceModel = new TargetedTrustEngagementSummaryResponse
		{
			TargetedTrustEngagementId = testData.Id,
			CaseUrn = testData.CaseUrn,
			Title = testData.Title,
			CreatedAt = testData.CreatedAt,
			UpdatedAt = testData.UpdatedAt,
			ClosedAt = null
		};

		// act
		var result = serviceModel.ToActionSummary();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Name, Is.EqualTo($"TTE - {testData.Title}"));
			Assert.That(result.ClosedDate, Is.EqualTo(""));
			Assert.That(result.OpenedDate, Is.EqualTo("05 September 2021"));
			Assert.That(result.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/targetedtrustengagement/{testData.Id}"));
			Assert.That(result.StatusName, Is.EqualTo("In progress"));
		});

		result.RawOpenedDate.Should().Be(testData.CreatedAt);
		result.RawClosedDate.Should().Be(null);
	}

	[Test]
	public void WhenMapToViewModel_IsClosed_ReturnsCorrectModel()
	{
		var response = _fixture.Create<GetTargetedTrustEngagementResponse>();
		response.ClosedAt = new DateTime(2022, 5, 7);

		var result = TargetedTrustEngagementMapping.ToViewModel(response, new GetCasePermissionsResponse());

		result.IsClosed.Should().BeTrue();
		result.DateClosed.Should().Be("07 May 2022");
	}

	[TestCaseSource(nameof(PermissionTestCases))]
	public void WhenMapToViewModel_IsNotEditable_ReturnsCorrectModel(DateTime? closedAt, GetCasePermissionsResponse permissionResponse)
	{
		var tteResponse = _fixture.Create<GetTargetedTrustEngagementResponse>();
		tteResponse.ClosedAt = closedAt;

		var result = TargetedTrustEngagementMapping.ToViewModel(tteResponse, permissionResponse);

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
			Title = _fixture.Create<string>(),
			CreatedAt = new DateTimeOffset(2021, 9, 5, 0, 0, 0, new TimeSpan()),
			ClosedAt = new DateTimeOffset(2024, 8, 30, 0, 0, 0, new TimeSpan()),
			UpdatedAt = _fixture.Create<DateTimeOffset>()
		};

		var serviceModel = new TargetedTrustEngagementSummaryResponse
		{
			TargetedTrustEngagementId = testData.Id,
			CaseUrn = testData.CaseUrn,
			Title = testData.Title,
			CreatedAt = testData.CreatedAt,
			UpdatedAt = testData.UpdatedAt,
			ClosedAt = testData.ClosedAt
		};

		// act
		var result = serviceModel.ToActionSummary();

		// assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Name, Is.EqualTo($"TTE - {testData.Title}"));

			Assert.That(result.ClosedDate, Is.EqualTo("30 August 2024"));
			Assert.That(result.OpenedDate, Is.EqualTo("05 September 2021"));
			Assert.That(result.RelativeUrl, Is.EqualTo($"/case/{testData.CaseUrn}/management/action/targetedtrustengagement/{testData.Id}"));
			Assert.That(result.StatusName, Is.EqualTo("Completed"));
		});

		result.RawOpenedDate.Should().Be(testData.CreatedAt);
		result.RawClosedDate.Should().Be(testData.ClosedAt);
	}
	
	private static IEnumerable<TestCaseData> PermissionTestCases()
	{
		yield return new TestCaseData(new DateTime(), new GetCasePermissionsResponse() { Permissions = new List<CasePermission>() { CasePermission.Edit } });
		yield return new TestCaseData(null, new GetCasePermissionsResponse());
	}
}