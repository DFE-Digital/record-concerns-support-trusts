using ConcernsCaseWork.Models;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class RecordMapping
	{
		public static RecordDto MapConcernType(PatchCaseModel patchCaseModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, patchCaseModel.UpdatedAt, recordDto.ReviewAt,
				recordDto.ClosedAt, patchCaseModel.Type, patchCaseModel.SubType, 
				recordDto.Reason, recordDto.CaseUrn, patchCaseModel.TypeUrn, recordDto.RatingUrn,
				recordDto.Urn, recordDto.StatusUrn);
		}
		
		public static RecordDto MapRiskRating(PatchCaseModel patchCaseModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, patchCaseModel.UpdatedAt, recordDto.ReviewAt,
				recordDto.ClosedAt, recordDto.Name, recordDto.Description, 
				recordDto.Reason, recordDto.CaseUrn, recordDto.TypeUrn, patchCaseModel.RatingUrn, 
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

		public static IList<RecordModel> MapDtoToModel(IList<RecordDto> recordsDto)
		{
			var recordsModel = new List<RecordModel>();
			if (recordsDto is null || !recordsDto.Any()) return recordsModel;

			recordsModel.AddRange(recordsDto.Select(recordDto => 
				new RecordModel(recordDto.CaseUrn, recordDto.TypeUrn, recordDto.RatingUrn, recordDto.Urn, recordDto.StatusUrn)));

			return recordsModel;
		}
	}
}