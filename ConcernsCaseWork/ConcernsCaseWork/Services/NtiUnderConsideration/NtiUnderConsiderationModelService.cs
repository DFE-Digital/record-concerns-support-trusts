using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiUnderConsideration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.NtiUnderConsideration
{
	public class NtiUnderConsiderationModelService : INtiUnderConsiderationModelService
	{
		private readonly INtiUnderConsiderationCachedService _ntiCachedService;
		private readonly INtiUnderConsiderationStatusesCachedService _ntiStatusesCachedService;

		public NtiUnderConsiderationModelService(INtiUnderConsiderationCachedService ntiCachedService, INtiUnderConsiderationStatusesCachedService ntiStatusesCachedService)
		{
			_ntiCachedService = ntiCachedService;
			_ntiStatusesCachedService = ntiStatusesCachedService;
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

			if (dto.ClosedStatusId.HasValue)
			{
				var statuses = await _ntiStatusesCachedService.GetAllStatuses();
				dto.ClosedStatusName = statuses.SingleOrDefault(s => s.Id == dto.ClosedStatusId)?
					.Name;
			}
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
