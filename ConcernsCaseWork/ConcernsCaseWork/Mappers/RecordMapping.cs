using ConcernsCaseWork.Models;
using Service.TRAMS.Records;

namespace ConcernsCaseWork.Mappers
{
	public static class RecordMapping
	{
		public static RecordDto Map(PatchCaseModel patchCaseModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, patchCaseModel.UpdatedAt, recordDto.ReviewAt,
				recordDto.ClosedAt, recordDto.Name, $"{patchCaseModel.RecordType}-{patchCaseModel.RecordSubType}", 
				recordDto.Reason, recordDto.CaseUrn, patchCaseModel.TypeUrn, recordDto.RatingUrn,
				recordDto.Primary, recordDto.Urn, recordDto.Status);
		}
	}
}