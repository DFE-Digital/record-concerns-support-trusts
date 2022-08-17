using ConcernsCasework.Service.Records;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Records
{
	public interface IRecordCachedService
	{
		Task<IList<RecordDto>> GetRecordsByCaseUrn(string caseworker, long caseUrn);
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker);
		Task PatchRecordByUrn(RecordDto recordDto, string caseworker);
	}
}