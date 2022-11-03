using ConcernsCaseWork.Service.Nti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Nti
{
	public interface INtiCachedService
	{
		public Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn);
		Task<NtiDto> CreateNtiAsync(NtiDto newNti);
		Task<NtiDto> GetNtiAsync(long ntiId);
		Task<NtiDto> PatchNtiAsync(NtiDto nti);
		Task SaveNtiAsync(NtiDto nti, string continuationId);
		Task<NtiDto> GetNtiAsync(string continuationId);
	}
}
