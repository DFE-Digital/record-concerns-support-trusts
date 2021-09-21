using Service.TRAMS.Cases;
using Service.TRAMS.Records;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Records
{
	public interface IRecordCachedService
	{
		Task<IList<RecordDto>> GetRecordsByCaseUrn(CaseDto caseDto);
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker);
		Task PatchRecordByUrn(RecordDto recordDto, string caseworker);
	}
}