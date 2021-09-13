using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordRatingHistory
{
	public interface IRecordRatingHistoryService
	{
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByCaseUrn(long caseUrn);
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByRecordUrn(long recordUrn);
		Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto);
	}
}