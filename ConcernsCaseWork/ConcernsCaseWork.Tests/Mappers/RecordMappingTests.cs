using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class RecordMappingTests
	{
		[Test]
		public void WhenMapRating_ReturnsRecordDto()
		{
			// arrange
			var patchRecordModel = RecordFactory.BuildPatchRecordModel();
			var record = RecordFactory.BuildRecordDto();
			
			// act
			var recordDto = RecordMapping.MapRating(patchRecordModel, record);

			// assert
			Assert.That(recordDto, Is.Not.Null);
			Assert.That(recordDto.Description, Is.EqualTo(record.Description));
			Assert.That(recordDto.Name, Is.EqualTo(record.Name));
			Assert.That(recordDto.Reason, Is.EqualTo(record.Reason));
			Assert.That(recordDto.StatusId, Is.EqualTo(record.StatusId));
			Assert.That(recordDto.Id, Is.EqualTo(record.Id));
			Assert.That(recordDto.CaseUrn, Is.EqualTo(record.CaseUrn));
			Assert.That(recordDto.ClosedAt, Is.EqualTo(record.ClosedAt));
			Assert.That(recordDto.CreatedAt, Is.EqualTo(record.CreatedAt));
			Assert.That(recordDto.RatingId, Is.EqualTo(patchRecordModel.RatingId));
			Assert.That(recordDto.ReviewAt, Is.EqualTo(record.ReviewAt));
			Assert.That(recordDto.TypeId, Is.EqualTo(record.TypeId));
			Assert.That(recordDto.UpdatedAt, Is.EqualTo(patchRecordModel.UpdatedAt));
		}

		[Test]
		public void WhenMapClosure_ReturnsRecordDto()
		{
			// arrange
			var patchRecordModel = RecordFactory.BuildPatchRecordModel();
			var record = RecordFactory.BuildRecordDto();
			var statusDto = StatusFactory.BuildStatusDto(StatusEnum.Live.ToString(), 1);

			// act
			var recordDto = RecordMapping.MapClosure(patchRecordModel, record, statusDto);

			// assert
			Assert.That(recordDto, Is.Not.Null);
			Assert.That(recordDto.Description, Is.EqualTo(record.Description));
			Assert.That(recordDto.Name, Is.EqualTo(record.Name));
			Assert.That(recordDto.Reason, Is.EqualTo(record.Reason));
			Assert.That(recordDto.StatusId, Is.EqualTo(record.StatusId));
			Assert.That(recordDto.Id, Is.EqualTo(record.Id));
			Assert.That(recordDto.CaseUrn, Is.EqualTo(record.CaseUrn));
			Assert.That(recordDto.ClosedAt, Is.EqualTo(record.ClosedAt));
			Assert.That(recordDto.CreatedAt, Is.EqualTo(record.CreatedAt));
			Assert.That(recordDto.RatingId, Is.EqualTo(record.RatingId));
			Assert.That(recordDto.ReviewAt, Is.EqualTo(record.ReviewAt));
			Assert.That(recordDto.TypeId, Is.EqualTo(record.TypeId));
			Assert.That(recordDto.UpdatedAt, Is.EqualTo(patchRecordModel.UpdatedAt));
		}

		[Test]
		public void WhenMapClosure_When_ClosedDate_IsNotNull_ReturnsRecordDto()
		{
			// arrange
			var patchRecordModel = RecordFactory.BuildPatchRecordModel();
			patchRecordModel.ClosedAt = DateTimeOffset.Now;
			var record = RecordFactory.BuildRecordDto();
			var statusDto = StatusFactory.BuildStatusDto(StatusEnum.Close.ToString(), 3);

			// act
			var recordDto = RecordMapping.MapClosure(patchRecordModel, record, statusDto);

			// assert
			Assert.That(recordDto, Is.Not.Null);
			Assert.That(recordDto.Description, Is.EqualTo(record.Description));
			Assert.That(recordDto.Name, Is.EqualTo(record.Name));
			Assert.That(recordDto.Reason, Is.EqualTo(record.Reason));
			Assert.That(recordDto.StatusId, Is.EqualTo(statusDto.Id));
			Assert.That(recordDto.Id, Is.EqualTo(record.Id));
			Assert.That(recordDto.CaseUrn, Is.EqualTo(record.CaseUrn));
			Assert.That(recordDto.ClosedAt, Is.EqualTo(patchRecordModel.ClosedAt));
			Assert.That(recordDto.CreatedAt, Is.EqualTo(record.CreatedAt));
			Assert.That(recordDto.RatingId, Is.EqualTo(record.RatingId));
			Assert.That(recordDto.ReviewAt, Is.EqualTo(record.ReviewAt));
			Assert.That(recordDto.TypeId, Is.EqualTo(record.TypeId));
			Assert.That(recordDto.UpdatedAt, Is.EqualTo(patchRecordModel.UpdatedAt));
		}

		[Test]
		public void WhenMapDtoToCreateRecordModel_ReturnsCreateRecordsModel()
		{
			// arrange
			var recordsDto = RecordFactory.BuildListRecordDto();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();

			// act
			var createRecordsDto = RecordMapping.MapDtoToCreateRecordModel(recordsDto, typesDto, ratingsDto);

			// assert
			Assert.NotNull(createRecordsDto);
			Assert.That(createRecordsDto.Count, Is.EqualTo(recordsDto.Count));
			
			for (var index = 0; index < createRecordsDto.Count; ++index)
			{
				var recordDto = recordsDto.ElementAt(index);
				var typeModel = TypeMapping.MapDtoToModel(typesDto, recordDto.TypeId);
				var ratingModel = RatingMapping.MapDtoToModel(ratingsDto, recordDto.RatingId);
				
				Assert.That(createRecordsDto.ElementAt(index).Type, Is.EqualTo(typeModel.Type));
				Assert.That(createRecordsDto.ElementAt(index).CaseUrn, Is.EqualTo(recordDto.CaseUrn));
				Assert.That(createRecordsDto.ElementAt(index).RagRating, Is.EqualTo(ratingModel.RagRating));
				Assert.That(createRecordsDto.ElementAt(index).RatingName, Is.EqualTo(ratingModel.Name));
				Assert.That(createRecordsDto.ElementAt(index).RatingId, Is.EqualTo(recordDto.RatingId));
				Assert.That(createRecordsDto.ElementAt(index).SubType, Is.EqualTo(typeModel.SubType));
				Assert.That(createRecordsDto.ElementAt(index).TypeId, Is.EqualTo(recordDto.TypeId));
				Assert.That(createRecordsDto.ElementAt(index).TypeDisplay, Is.EqualTo(typeModel.TypeDisplay));
				Assert.That(createRecordsDto.ElementAt(index).RagRatingCss, Is.EqualTo(ratingModel.RagRatingCss));
			}
		}
		
		[Test]
		public void WhenMapDtoToCreateRecordModel_RecordsDtoIsNull_ReturnsEmptyCreateRecordsModel()
		{
			// arrange
			var recordsDto = new List<RecordDto>();
			var typesDto = TypeFactory.BuildListTypeDto();
			var ratingsDto = RatingFactory.BuildListRatingDto();

			// act
			var createRecordsDto = RecordMapping.MapDtoToCreateRecordModel(null, typesDto, ratingsDto);

			// assert
			Assert.NotNull(createRecordsDto);
			Assert.That(createRecordsDto.Count, Is.EqualTo(recordsDto.Count));
		}
	}
}