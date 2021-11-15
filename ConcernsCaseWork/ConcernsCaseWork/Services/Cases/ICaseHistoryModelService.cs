using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseHistoryModelService
	{
		Task<IList<CaseHistoryModel>> GetCasesHistory(string caseworker, long caseUrn);
	}
}