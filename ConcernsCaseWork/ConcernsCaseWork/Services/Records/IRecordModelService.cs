using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Records;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Records
{
	public interface IRecordModelService
	{
		Task<IList<RecordModel>> GetRecordsModelByCaseUrn(long caseUrn);

		Task<RecordModel> GetRecordModelById(long caseUrn, long id);

		Task PatchRecordStatus(PatchRecordModel patchRecordModel);
		
		Task<IList<CreateRecordModel>> GetCreateRecordsModelByCaseUrn(long caseUrn);
		
		Task<RecordDto> PostRecordByCaseUrn(CreateRecordModel createRecordModel);
	}
}
