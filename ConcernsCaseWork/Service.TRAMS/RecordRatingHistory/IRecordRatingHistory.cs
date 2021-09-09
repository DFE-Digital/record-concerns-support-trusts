using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Service.TRAMS.RecordRatingHistory
{
	public interface IRecordRatingHistory
	{
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByCaseUrn(BigInteger caseUrn);
		Task<IList<RecordRatingHistoryDto>> GetRecordsRatingHistoryByRecordUrn(BigInteger recordUrn);
		Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto);
	}
}