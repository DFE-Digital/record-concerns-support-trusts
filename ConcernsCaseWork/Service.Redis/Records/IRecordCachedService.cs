using Service.TRAMS.Records;
using System.Threading.Tasks;

namespace Service.Redis.Records
{
	public interface IRecordCachedService
	{
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker);
	}
}