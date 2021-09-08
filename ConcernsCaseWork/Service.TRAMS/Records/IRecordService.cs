using Service.TRAMS.Dto;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Service.TRAMS.Records
{
	public interface IRecordService
	{
		Task<IList<RecordDto>> GetRecordsByCaseUrn(BigInteger caseUrn);
		Task<RecordDto> PostRecordByCaseUrn(RecordDto recordDto);
		Task<RecordDto> PatchRecordByUrn(RecordDto recordDto);
	}
}