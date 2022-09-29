using ConcernsCaseWork.Models;
using Service.Redis.Models;
using Service.TRAMS.Ratings;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Types;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class RecordMapping
	{
		public static RecordDto MapRating(PatchRecordModel patchRecordModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, 
				patchRecordModel.UpdatedAt, 
				recordDto.ReviewAt,
				recordDto.ClosedAt, 
				recordDto.Name, 
				recordDto.Description, 
				recordDto.Reason, 
				recordDto.CaseUrn, 
				recordDto.TypeId, 
				patchRecordModel.RatingId, 
				recordDto.Id, 
				recordDto.StatusId);
		}
		
		public static RecordDto MapClosure(PatchRecordModel patchRecordModel, RecordDto recordDto, StatusDto statusDto)
		{
			return new RecordDto(recordDto.CreatedAt,
				patchRecordModel.UpdatedAt,
				recordDto.ReviewAt,
				patchRecordModel.ClosedAt ?? recordDto.ClosedAt, 
				recordDto.Name, recordDto.Description, 
				recordDto.Reason, recordDto.CaseUrn, 
				recordDto.TypeId, recordDto.RatingId,
				recordDto.Id, statusDto.Id);
		}

		public static IList<RecordModel> MapDtoToModel(IList<RecordDto> recordsDto, 
			IList<TypeDto> typesDto, 
			IList<RatingDto> ratingsDto,
			IList<StatusDto> statusesDto)
		{
			var recordsModel = new List<RecordModel>();
			if (recordsDto is null || !recordsDto.Any()) return recordsModel;
			
			recordsModel.AddRange(recordsDto.Select(recordDto =>
			{
				var recordModel = new RecordModel(
					recordDto.CaseUrn,
					recordDto.TypeId,
					TypeMapping.MapDtoToModel(typesDto, recordDto.TypeId),
					recordDto.RatingId,
					RatingMapping.MapDtoToModel(ratingsDto, recordDto.RatingId),
					recordDto.Id,
					recordDto.StatusId,
					StatusMapping.MapDtoToModel(statusesDto, recordDto.StatusId));

				return recordModel;
			}));

			return recordsModel;
		}
		
		public static IList<CreateRecordModel> MapDtoToCreateRecordModel(IList<RecordDto> recordsDto, 
			IList<TypeDto> typesDto, 
			IList<RatingDto> ratingsDto)
		{
			var createRecordsModel = new List<CreateRecordModel>();
			if (recordsDto is null || !recordsDto.Any()) return createRecordsModel;
		
			createRecordsModel.AddRange(recordsDto.Select(recordDto =>
			{
				var typeModel = TypeMapping.MapDtoToModel(typesDto, recordDto.TypeId);
				var ratingModel = RatingMapping.MapDtoToModel(ratingsDto, recordDto.RatingId);
				
				var createRecordModel = new CreateRecordModel
				{
					CaseUrn = recordDto.CaseUrn,
					TypeId = recordDto.TypeId,
					Type = typeModel.Type,
					SubType = typeModel.SubType,
					RatingId = recordDto.RatingId,
					RatingName = ratingModel.Name,
					RagRating = ratingModel.RagRating,
					RagRatingCss = ratingModel.RagRatingCss
				};

				return createRecordModel;
			}));
			
			return createRecordsModel;
		}
	}
}