using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface INtiUnderConsiderationModelService
	{
		public Task<NtiModel> CreateNti(NtiModel nti);
		public Task<IEnumerable<NtiModel>> GetNtiUnderConsiderationsForCase(long caseUrn);
		public Task<NtiModel> GetNtiUnderConsideration(long ntiUcId);
		public Task<NtiModel> PatchNtiUnderConsideration(NtiModel nti);
	}
}
