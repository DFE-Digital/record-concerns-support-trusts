using ConcernsCaseWork.Models;
using Service.TRAMS.Records;

namespace ConcernsCaseWork.Mappers
{
	public static class RecordMapping
	{
		public static RecordDto MapConcernType(PatchCaseModel patchCaseModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, patchCaseModel.UpdatedAt, recordDto.ReviewAt,
				recordDto.ClosedAt, recordDto.Name, $"{patchCaseModel.RecordType} {patchCaseModel.RecordSubType}", 
				recordDto.Reason, recordDto.CaseUrn, patchCaseModel.TypeUrn, recordDto.RatingUrn,
				recordDto.Primary, recordDto.Urn, recordDto.Status);
		}
		
		public static RecordDto MapRiskRating(PatchCaseModel patchCaseModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, patchCaseModel.UpdatedAt, recordDto.ReviewAt,
				recordDto.ClosedAt, recordDto.Name, recordDto.Description, 
				recordDto.Reason, recordDto.CaseUrn, recordDto.TypeUrn, patchCaseModel.RatingUrn,
				recordDto.Primary, recordDto.Urn, recordDto.Status);
		}
	}
}