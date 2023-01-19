using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.NtiUnderConsideration;
using ConcernsCaseWork.Service.NtiUnderConsideration;
using ConcernsCaseWork.Service.Permissions;
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
		private readonly ICasePermissionsService _casePermissionsService;

		public NtiUnderConsiderationModelService(
			INtiUnderConsiderationCachedService ntiCachedService, 
			INtiUnderConsiderationStatusesCachedService ntiStatusesCachedService,
			ICasePermissionsService casePermissionsService)
		{
			_ntiCachedService = ntiCachedService;
			_ntiStatusesCachedService = ntiStatusesCachedService;
			_casePermissionsService = casePermissionsService;
		}

		public async Task<NtiUnderConsiderationModel> CreateNti(NtiUnderConsiderationModel nti)
		{
			nti.CreatedAt = DateTime.Now;
			nti.UpdatedAt = nti.CreatedAt;
			var dto = await _ntiCachedService.CreateNti(NtiUnderConsiderationMappers.ToDBModel(nti));
			return NtiUnderConsiderationMappers.ToServiceModel(dto);
		}

		public async Task<NtiUnderConsiderationModel> GetNtiUnderConsiderationViewModel(long caseId, long ntiUcId)
		{
			var dto = await GetNtiUnderConsiderationDto(ntiUcId);
			var permissionResponse = await _casePermissionsService.GetCasePermissions(caseId);

			return NtiUnderConsiderationMappers.ToServiceModel(dto, permissionResponse);
		}

		public async Task<NtiUnderConsiderationModel> GetNtiUnderConsideration(long ntiUcId)
		{
			var dto = await GetNtiUnderConsiderationDto(ntiUcId);

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

		private async Task<NtiUnderConsiderationDto> GetNtiUnderConsiderationDto(long ntiUcId)
		{
			var result = await _ntiCachedService.GetNti(ntiUcId);

			if (result.ClosedStatusId.HasValue)
			{
				var statuses = await _ntiStatusesCachedService.GetAllStatuses();
				result.ClosedStatusName = statuses.SingleOrDefault(s => s.Id == result.ClosedStatusId)?
					.Name;
			}

			return result;
		}
	}
}
