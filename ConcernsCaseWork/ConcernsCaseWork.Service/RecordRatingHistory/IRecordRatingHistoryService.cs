namespace ConcernsCaseWork.Service.RecordRatingHistory
{
	public interface IRecordRatingHistoryService
	{
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByCaseUrn(long caseUrn);
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByRecordUrn(long recordUrn);
		Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto);
	}
}