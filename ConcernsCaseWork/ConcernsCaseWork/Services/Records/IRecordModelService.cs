using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Records
{
	public interface IRecordModelService
	{
		Task<IList<RecordModel>> GetRecordsModelByCaseUrn(string caseworker, long caseUrn);

		Task<RecordModel> GetRecordModelByUrn(string caseworker, long caseUrn, long urn);
	}
}
