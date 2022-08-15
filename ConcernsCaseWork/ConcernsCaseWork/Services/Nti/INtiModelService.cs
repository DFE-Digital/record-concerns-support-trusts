using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Nti
{
	public interface INtiModelService
	{
		public Task<ICollection<NtiModel>> GetNtisForCase(long caseUrn);
	}
}
