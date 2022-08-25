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

		public NtiModelService(INtiCachedService ntiCachedService)
		{
			_ntiCachedService = ntiCachedService;
		}

		public async Task<NtiModel> CreateNtiAsync(NtiModel ntiModel)
		{
			var created = await _ntiCachedService.CreateNtiAsync(NtiMappers.ToDBModel(ntiModel));
			return NtiMappers.ToServiceModel(created);
		}

		public async Task<NtiModel> GetNtiAsync(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			var dto = await _ntiCachedService.GetNtiAsync(continuationId);
			return NtiMappers.ToServiceModel(dto);
		}

		public async Task<NtiModel> GetNtiByIdAsync(long ntiId)
		{
			var dto = await _ntiCachedService.GetNtiAsync(ntiId);
			return NtiMappers.ToServiceModel(dto);
		}

		public async Task<ICollection<NtiModel>> GetNtisForCaseAsync(long caseUrn)
		{
			var ntiDtos = await _ntiCachedService.GetNtisForCaseAsync(caseUrn);
			return ntiDtos.Select(dto => NtiMappers.ToServiceModel(dto)).ToArray();
		}

		public async Task<NtiModel> PatchNtiAsync(NtiModel patchNti)
		{
			patchNti.UpdatedAt = DateTime.Now;
			var dto = NtiMappers.ToDBModel(patchNti);
			var patchedDto = await _ntiCachedService.PatchNtiAsync(dto);

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
	}
}
