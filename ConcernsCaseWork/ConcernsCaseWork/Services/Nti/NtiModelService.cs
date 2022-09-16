using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.Nti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Nti
{
	public class NtiModelService : INtiModelService
	{
		private readonly INtiCachedService _ntiCachedService;
		private readonly INtiStatusesCachedService _ntiStatusesCachedService;

		public NtiModelService(INtiCachedService ntiCachedService, INtiStatusesCachedService ntiStatusesCachedService)
		{
			_ntiCachedService = ntiCachedService;
			_ntiStatusesCachedService = ntiStatusesCachedService;
		}

		public async Task<NtiModel> CreateNtiAsync(NtiModel ntiModel)
		{
			var created = await _ntiCachedService.CreateNtiAsync(NtiMappers.ToDBModel(ntiModel));
			var statuses = await _ntiStatusesCachedService.GetAllStatusesAsync();

			return NtiMappers.ToServiceModel(created, statuses);
		}

		public async Task<NtiModel> GetNtiAsync(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			var dto = await _ntiCachedService.GetNtiAsync(continuationId);
			var statuses = await _ntiStatusesCachedService.GetAllStatusesAsync();

			return NtiMappers.ToServiceModel(dto, statuses);
		}

		public async Task<NtiModel> GetNtiByIdAsync(long ntiId)
		{
			var dto = await _ntiCachedService.GetNtiAsync(ntiId);
			var statuses = await _ntiStatusesCachedService.GetAllStatusesAsync();

			return NtiMappers.ToServiceModel(dto, statuses);
		}

		public async Task<ICollection<NtiModel>> GetNtisForCaseAsync(long caseUrn)
		{
			var ntiDtos = await _ntiCachedService.GetNtisForCaseAsync(caseUrn);
			var statuses = await _ntiStatusesCachedService.GetAllStatusesAsync();

			return ntiDtos.Select(dto => NtiMappers.ToServiceModel(dto, statuses)).ToArray();
		}

		public async Task<NtiModel> PatchNtiAsync(NtiModel patchNti)
		{
			patchNti.UpdatedAt = DateTime.Now;
			var dto = NtiMappers.ToDBModel(patchNti);
			var patchedDto = await _ntiCachedService.PatchNtiAsync(dto);
			var statuses = await _ntiStatusesCachedService.GetAllStatusesAsync();

			return NtiMappers.ToServiceModel(patchedDto, statuses);
		}

		public async Task StoreNtiAsync(NtiModel ntiModel, string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			await _ntiCachedService.SaveNtiAsync(NtiMappers.ToDBModel(ntiModel), continuationId);
		}
	}
}
