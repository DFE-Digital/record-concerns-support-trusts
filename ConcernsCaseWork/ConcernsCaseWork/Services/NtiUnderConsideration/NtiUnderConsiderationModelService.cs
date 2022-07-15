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

		public async Task<NtiUnderConsiderationModel> CreateNti(NtiUnderConsiderationModel nti)
		{
			nti.CreatedAt = DateTime.Now;
			nti.UpdatedAt = nti.CreatedAt;
			var dto = await _ntiCachedService.CreateNti(NtiUnderConsiderationMappers.ToDBModel(nti));
			return NtiUnderConsiderationMappers.ToServiceModel(dto);
		}

		public async Task<NtiUnderConsiderationModel> GetNtiUnderConsideration(long ntiUcId)
		{
			var dto = await _ntiCachedService.GetNti(ntiUcId);
			return NtiUnderConsiderationMappers.ToServiceModel(dto);
		}

		public async Task<IEnumerable<NtiUnderConsiderationModel>> GetNtiUnderConsiderationsForCase(long caseUrn)
		{
			var ntis = await _ntiCachedService.GetNtisForCase(caseUrn);
			return ntis.Select(nti => NtiUnderConsiderationMappers.ToServiceModel(nti));
		}

		public async Task<NtiUnderConsiderationModel> PatchNtiUnderConsideration(NtiUnderConsiderationModel nti)
		{
			nti.UpdatedAt = DateTime.Now;
			var patched = await _ntiCachedService.PatchNti(NtiUnderConsiderationMappers.ToDBModel(nti));
			return NtiUnderConsiderationMappers.ToServiceModel(patched);
		}
	}
}
