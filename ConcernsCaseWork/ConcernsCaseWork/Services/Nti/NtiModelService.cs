using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.Nti;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Nti
{
	public class NtiModelService : INtiModelService
	{
		private readonly INtiCachedService _ntiCachedService;

		public NtiModelService(INtiCachedService ntiCachedService)
		{
			_ntiCachedService = ntiCachedService;
		}

		public async Task<ICollection<NtiModel>> GetNtisForCase(long caseUrn)
		{
			var ntiDtos = await _ntiCachedService.GetNtisForCase(caseUrn);
			return ntiDtos.Select(dto => NtiMappers.ToServiceModel(dto)).ToArray();
		}
	}
}
