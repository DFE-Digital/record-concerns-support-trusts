using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
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
		private readonly INtiUnderConsiderationService _ntiUnderConsiderationService;
		private readonly ICasePermissionsService _casePermissionsService;

		public NtiUnderConsiderationModelService(
			INtiUnderConsiderationService ntiUnderConsiderationService,
			ICasePermissionsService casePermissionsService)
		{
			_ntiUnderConsiderationService = ntiUnderConsiderationService;
			_casePermissionsService = casePermissionsService;
		}

		public async Task<NtiUnderConsiderationModel> CreateNti(NtiUnderConsiderationModel nti)
		{
			nti.CreatedAt = DateTime.Now;
			nti.UpdatedAt = nti.CreatedAt;
			var dto = await _ntiUnderConsiderationService.CreateNti(NtiUnderConsiderationMappers.ToDBModel(nti));
			return NtiUnderConsiderationMappers.ToServiceModel(dto);
		}

		public async Task<NtiUnderConsiderationModel> GetNtiUnderConsiderationViewModel(long caseId, long ntiUcId)
		{
			var dto = await _ntiUnderConsiderationService.GetNti(ntiUcId);
			var permissionResponse = await _casePermissionsService.GetCasePermissions(caseId);

			return NtiUnderConsiderationMappers.ToServiceModel(dto, permissionResponse);
		}

		public async Task<NtiUnderConsiderationModel> GetNtiUnderConsideration(long ntiUcId)
		{
			var dto = await _ntiUnderConsiderationService.GetNti(ntiUcId);

			return NtiUnderConsiderationMappers.ToServiceModel(dto);
		}

		public async Task<IEnumerable<NtiUnderConsiderationModel>> GetNtiUnderConsiderationsForCase(long caseUrn)
		{
			var ntis = await _ntiUnderConsiderationService.GetNtisForCase(caseUrn);
			return ntis.Select(nti => NtiUnderConsiderationMappers.ToServiceModel(nti));
		}

		public async Task<NtiUnderConsiderationModel> PatchNtiUnderConsideration(NtiUnderConsiderationModel nti)
		{
			nti.UpdatedAt = DateTime.Now;
			var patched = await _ntiUnderConsiderationService.PatchNti(NtiUnderConsiderationMappers.ToDBModel(nti));
			return NtiUnderConsiderationMappers.ToServiceModel(patched);
		}

		public async Task DeleteNtiUnderConsideration(long ntiUcId)
		{
			await _ntiUnderConsiderationService.DeleteNti(ntiUcId);
		}
	}
}
