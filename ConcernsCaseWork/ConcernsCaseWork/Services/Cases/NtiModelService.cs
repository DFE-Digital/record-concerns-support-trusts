using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.Nti;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public class NtiModelService : INtiModelService
	{
		private readonly INtiCachedService _ntiCachedService;

		public NtiModelService(INtiCachedService ntiCachedService)
		{
			_ntiCachedService = ntiCachedService;
		}

		public async Task<NtiModel> CreateNti(NtiModel nti)
		{
			var created = await _ntiCachedService.CreateNti(NtiMappers.ToDBModel(nti));
		}

		public Task<IEnumerable<NtiModel>> GetNtiUnderConsiderationsForCase(long caseUrn)
		{

			return Task.FromResult(Enumerable.Empty<NtiModel>());
		}
	}
}
