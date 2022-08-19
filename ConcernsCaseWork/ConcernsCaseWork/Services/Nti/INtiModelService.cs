using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Nti
{
	public interface INtiModelService
	{
		Task<ICollection<NtiModel>> GetNtisForCase(long caseUrn);
		Task<NtiModel> CreateNtiAsync(NtiModel ntiModel);
		Task<NtiModel> GetNtiById(long ntiId);
		Task<NtiModel> PatchNtiAsync(NtiModel patchNti);
		Task<NtiModel> GetNtiAsync(string continuationId);
		Task StoreNti(NtiModel ntiModel, string continuationId);


	}
}
