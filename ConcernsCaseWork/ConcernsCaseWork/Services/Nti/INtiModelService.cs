using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Nti
{
	public interface INtiModelService
	{
		Task<ICollection<NtiModel>> GetNtisForCaseAsync(long caseUrn);
		Task<NtiModel> CreateNtiAsync(NtiModel ntiModel);
		Task<NtiModel> GetNtiByIdAsync(long ntiId);

		Task<NtiModel> GetNtiViewModelAsync(long caseId, long ntiId);

		Task<NtiModel> PatchNtiAsync(NtiModel patchNti);
		Task<NtiModel> GetNtiAsync(string continuationId);
		Task StoreNtiAsync(NtiModel ntiModel, string continuationId);

	}
}
