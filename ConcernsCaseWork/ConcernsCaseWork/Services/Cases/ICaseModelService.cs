using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICaseModelService
	{
		Task<(IList<HomeUiModel>, IList<HomeUiModel>)> GetCasesByCaseworker(string caseworker);
		
	}
}