using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Service.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Nti
{
	public class NtiModelService : INtiModelService
	{
		private readonly INtiCachedService _ntiCachedService;
		private readonly ICasePermissionsService _casePermissionsService;
		private INtiService _ntiService;

		public NtiModelService(
			INtiCachedService ntiCachedService,
			INtiService ntiService,
			ICasePermissionsService casePermissionsService)
		{
			_ntiCachedService = ntiCachedService;
			_ntiService = ntiService;
			_casePermissionsService = casePermissionsService;
		}

		public async Task<NtiModel> CreateNtiAsync(NtiModel ntiModel)
		{
			var created = await _ntiService.CreateNtiAsync(NtiMappers.ToDBModel(ntiModel));

			return NtiMappers.ToServiceModel(created);
		}

		public async Task<NtiModel> GetNtiViewModelAsync(long caseId, long ntiId)
		{
			var dto = await _ntiService.GetNtiAsync(ntiId);
			var permissions = await _casePermissionsService.GetCasePermissions(caseId);

			var result = NtiMappers.ToServiceModel(dto, permissions);

			return result;
		}

		public async Task<NtiModel> GetNtiByIdAsync(long ntiId)
		{
			var dto = await _ntiService.GetNtiAsync(ntiId);

			return NtiMappers.ToServiceModel(dto);
		}

		public async Task<ICollection<NtiModel>> GetNtisForCaseAsync(long caseUrn)
		{
			var ntiDtos = await _ntiService.GetNtisForCaseAsync(caseUrn);

			return ntiDtos.Select(NtiMappers.ToServiceModel).ToArray();
		}

		public async Task<NtiModel> PatchNtiAsync(NtiModel patchNti)
		{
			patchNti.UpdatedAt = DateTime.Now;
			var dto = NtiMappers.ToDBModel(patchNti);
			var patchedDto = await _ntiService.PatchNtiAsync(dto);

			return NtiMappers.ToServiceModel(patchedDto);
		}

		public async Task StoreNtiAsync(NtiModel ntiModel, string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			await _ntiCachedService.SaveNtiAsync(NtiMappers.ToDBModel(ntiModel), continuationId);
		}

		public async Task<NtiModel> GetNtiFromCacheAsync(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			var dto = await _ntiCachedService.GetNtiAsync(continuationId);

			return NtiMappers.ToServiceModel(dto);
		}
	}
}
