using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface INtiUnderConsiderationModelService
	{
		public Task<NtiUnderConsiderationModel> CreateNti(NtiUnderConsiderationModel nti);
		public Task<IEnumerable<NtiUnderConsiderationModel>> GetNtiUnderConsiderationsForCase(long caseUrn);
		public Task<NtiUnderConsiderationModel> GetNtiUnderConsideration(long ntiUcId);
		public Task<NtiUnderConsiderationModel> PatchNtiUnderConsideration(NtiUnderConsiderationModel nti);
	}
}
