using Service.TRAMS.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordRatingHistory
{
	public interface IRecordRatingHistory
	{
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByCaseUrn(string caseUrn);
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByRecordUrn(string recordUrn);
		Task<RecordRatingHistoryDto> PostRecordRatingHistoryByRecordUrn(RecordRatingHistoryDto recordRatingHistoryDto);
	}
}