using ConcernsCaseWork.Models;
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
		public static RecordDto MapRiskRating(PatchRecordModel patchRecordModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, patchRecordModel.UpdatedAt, recordDto.ReviewAt,
				recordDto.ClosedAt, recordDto.Name, recordDto.Description, 
				recordDto.Reason, recordDto.CaseUrn, recordDto.TypeUrn, patchRecordModel.RatingUrn, 
				recordDto.Urn, recordDto.StatusUrn);
		}
		
		public static RecordDto MapClosure(PatchCaseModel patchCaseModel, RecordDto recordDto, StatusDto statusDto)
		{
			return new RecordDto(recordDto.CreatedAt, patchCaseModel.UpdatedAt,
				patchCaseModel.ReviewAt ?? recordDto.ReviewAt,
				patchCaseModel.ClosedAt ?? recordDto.ClosedAt, 
				recordDto.Name, recordDto.Description, 
				recordDto.Reason, recordDto.CaseUrn, 
				recordDto.TypeUrn, recordDto.RatingUrn,
				recordDto.Urn, statusDto.Urn);
		}
		
		public static IList<RecordModel> MapDtoToModel(IList<RecordDto> recordsDto, 
			IList<TypeDto> typesDto, 
			IList<RatingDto> ratingsDto)
		{
			var recordsModel = new List<RecordModel>();
			if (recordsDto is null || !recordsDto.Any()) return recordsModel;

			recordsModel.AddRange(recordsDto.Select(recordDto =>
			{
				var recordModel = new RecordModel(
					recordDto.CaseUrn,
					recordDto.TypeUrn,
					TypeMapping.MapDtoToModel(typesDto, recordDto.TypeUrn),
					recordDto.RatingUrn,
					RatingMapping.MapDtoToModel(ratingsDto, recordDto.RatingUrn),
					recordDto.Urn,
					recordDto.StatusUrn);

				return recordModel;
			}));

			return recordsModel;
		}
	}
}