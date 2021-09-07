using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Records
{
	public interface IRecordService
	{
		Task<IList<RecordDto>> GetRecordsByCaseUrn(string caseUrn);
		Task<RecordDto> PostRecordByCaseUrn(RecordDto recordDto);
		Task<RecordDto> PatchRecordByUrn(RecordDto recordDto);
	}
}