﻿using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class RecordMappingTests
	{
		[Test]
		public void WhenMapConcernType_ReturnsRecordDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var record = RecordFactory.BuildRecordDto();
			
			// act
			var recordDto = RecordMapping.MapConcernType(patchCaseModel, record);

			// assert
			Assert.That(recordDto, Is.Not.Null);
			Assert.That(recordDto.Description, Is.EqualTo($"{patchCaseModel.CaseType} {patchCaseModel.CaseSubType}"));
			Assert.That(recordDto.Name, Is.EqualTo(record.Name));
			Assert.That(recordDto.Primary, Is.EqualTo(record.Primary));
			Assert.That(recordDto.Reason, Is.EqualTo(record.Reason));
			Assert.That(recordDto.Status, Is.EqualTo(record.Status));
			Assert.That(recordDto.Urn, Is.EqualTo(record.Urn));
			Assert.That(recordDto.CaseUrn, Is.EqualTo(record.CaseUrn));
			Assert.That(recordDto.ClosedAt, Is.EqualTo(record.ClosedAt));
			Assert.That(recordDto.CreatedAt, Is.EqualTo(record.CreatedAt));
			Assert.That(recordDto.RatingUrn, Is.EqualTo(record.RatingUrn));
			Assert.That(recordDto.ReviewAt, Is.EqualTo(record.ReviewAt));
			Assert.That(recordDto.TypeUrn, Is.EqualTo(patchCaseModel.TypeUrn));
			Assert.That(recordDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
		}
		
		[Test]
		public void WhenMapRiskRating_ReturnsRecordDto()
		{
			// arrange
			var patchCaseModel = CaseFactory.BuildPatchCaseModel();
			var record = RecordFactory.BuildRecordDto();
			
			// act
			var recordDto = RecordMapping.MapRiskRating(patchCaseModel, record);

			// assert
			Assert.That(recordDto, Is.Not.Null);
			Assert.That(recordDto.Description, Is.EqualTo(record.Description));
			Assert.That(recordDto.Name, Is.EqualTo(record.Name));
			Assert.That(recordDto.Primary, Is.EqualTo(record.Primary));
			Assert.That(recordDto.Reason, Is.EqualTo(record.Reason));
			Assert.That(recordDto.Status, Is.EqualTo(record.Status));
			Assert.That(recordDto.Urn, Is.EqualTo(record.Urn));
			Assert.That(recordDto.CaseUrn, Is.EqualTo(record.CaseUrn));
			Assert.That(recordDto.ClosedAt, Is.EqualTo(record.ClosedAt));
			Assert.That(recordDto.CreatedAt, Is.EqualTo(record.CreatedAt));
			Assert.That(recordDto.RatingUrn, Is.EqualTo(patchCaseModel.RatingUrn));
			Assert.That(recordDto.ReviewAt, Is.EqualTo(record.ReviewAt));
			Assert.That(recordDto.TypeUrn, Is.EqualTo(record.TypeUrn));
			Assert.That(recordDto.UpdatedAt, Is.EqualTo(patchCaseModel.UpdatedAt));
		}
	}
}