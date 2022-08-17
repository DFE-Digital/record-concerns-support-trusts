using ConcernsCaseWork.Models;
using Service.Redis.Models;
using ConcernsCasework.Service.Records;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Records
{
	public interface IRecordModelService
	{
		Task<IList<RecordModel>> GetRecordsModelByCaseUrn(string caseworker, long caseUrn);

		Task<RecordModel> GetRecordModelByUrn(string caseworker, long caseUrn, long urn);

		Task PatchRecordStatus(PatchRecordModel patchRecordModel);
		
		Task<IList<CreateRecordModel>> GetCreateRecordsModelByCaseUrn(string caseworker, long caseUrn);
		
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordModel createRecordModel, string caseworker);
	}
}
