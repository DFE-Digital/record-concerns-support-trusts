using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Records
{
	public interface IRecordService
	{
		Task<IList<RecordDto>> GetRecordsByCaseUrn(long caseUrn);
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto);
		Task<RecordDto> PatchRecordById(RecordDto recordDto);
	}
}