using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		// TODO - for example only...
		Task<IList<CaseModel>> GetCasesByCaseworker(string caseworker);
	}
}