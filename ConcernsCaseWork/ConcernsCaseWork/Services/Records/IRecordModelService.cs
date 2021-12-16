using ConcernsCaseWork.Models;
using Service.Redis.Models;
using Service.TRAMS.Records;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Records
{
	public interface IRecordModelService
	{
		Task<IList<RecordModel>> GetRecordsModelByCaseUrn(string caseworker, long caseUrn);

		Task<RecordModel> GetRecordModelByUrn(string caseworker, long caseUrn, long urn);
		
		Task<IList<CreateRecordModel>> GetCreateRecordsModelByCaseUrn(string caseworker, long caseUrn);
		
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordModel createRecordModel, string caseworker);
	}
}
