using ConcernsCaseWork.Models;
using Service.TRAMS.Records;
using Service.TRAMS.Status;

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
	}
}