using Service.TRAMS.RecordRatingHistory;
using System.Numerics;
using System.Threading.Tasks;

namespace Service.Redis.RecordRatingHistory
{
	public interface IRecordRatingHistoryCachedService
	{
		Task<RecordRatingHistoryDto> PostRecordRatingHistory(RecordRatingHistoryDto recordRatingHistoryDto, string caseworker, BigInteger caseUrn);
	}
}