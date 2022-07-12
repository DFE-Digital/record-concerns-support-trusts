using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiUnderConsideration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public class NtiUnderConsiderationModelService : INtiUnderConsiderationModelService
	{
		private readonly INtiUnderConsiderationCachedService _ntiCachedService;

		public NtiUnderConsiderationModelService(INtiUnderConsiderationCachedService ntiCachedService)
		{
			_ntiCachedService = ntiCachedService;
		}

		public async Task<NtiModel> CreateNti(NtiModel nti)
		{
			nti.CreatedAt = DateTime.Now;
			nti.UpdatedAt = nti.CreatedAt;
			var dto = await _ntiCachedService.CreateNti(NtiMappers.ToDBModel(nti));
			return NtiMappers.ToServiceModel(dto);
		}

		public async Task<NtiModel> GetNtiUnderConsideration(long ntiUcId)
		{
			var dto = await _ntiCachedService.GetNti(ntiUcId);
			return NtiMappers.ToServiceModel(dto);
		}

		public async Task<IEnumerable<NtiModel>> GetNtiUnderConsiderationsForCase(long caseUrn)
		{
			var ntis = await _ntiCachedService.GetNtisForCase(caseUrn);
			return ntis.Select(nti => NtiMappers.ToServiceModel(nti));
		}

		public async Task<NtiModel> PatchNtiUnderConsideration(NtiModel nti)
		{
			nti.UpdatedAt = DateTime.Now;
			var patched = await _ntiCachedService.PatchNti(NtiMappers.ToDBModel(nti));
			return NtiMappers.ToServiceModel(patched);
		}
	}
}
