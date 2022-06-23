using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public class NtiUnderConsiderationModelService : INtiUnderConsiderationModelService
	{
		public Task<IEnumerable<NtiUnderConsiderationModel>> GetNtiUnderConsiderationsForCase(long caseUrn)
		{
			return Task.FromResult(Enumerable.Empty<NtiUnderConsiderationModel>());
		}
	}
}
