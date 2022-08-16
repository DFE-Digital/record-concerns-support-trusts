using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiWarningLetter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.NtiWarningLetter
{
	public class NtiWarningLetterModelService : INtiWarningLetterModelService
	{
		private readonly INtiWarningLetterCachedService _ntiWarningLetterCachedService;
		private readonly INtiWarningLetterStatusesCachedService _ntiWarningLetterStatusesCachedService;

		public NtiWarningLetterModelService(INtiWarningLetterCachedService ntiWarningLetterCachedService, INtiWarningLetterStatusesCachedService ntiWarningLetterStatusService)
		{
			_ntiWarningLetterCachedService = ntiWarningLetterCachedService;
			_ntiWarningLetterStatusesCachedService = ntiWarningLetterStatusService;
		}

		public async Task<NtiWarningLetterModel> CreateNtiWarningLetter(NtiWarningLetterModel ntiWarningLetterModel)
		{
			var created = await _ntiWarningLetterCachedService.CreateNtiWarningLetterAsync(NtiWarningLetterMappers.ToDBModel(ntiWarningLetterModel));
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();

			return NtiWarningLetterMappers.ToServiceModel(created, statuses);
		}

		public async Task<IEnumerable<NtiWarningLetterModel>> GetNtiWarningLettersForCase(long caseUrn)
		{
			var dtos = await _ntiWarningLetterCachedService.GetNtiWarningLettersForCaseAsync(caseUrn);
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();


			return dtos?.Select(dto => NtiWarningLetterMappers.ToServiceModel(dto, statuses)).ToArray();
		}


		public async Task<NtiWarningLetterModel> GetWarningLetter(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			var dto = await _ntiWarningLetterCachedService.GetNtiWarningLetter(continuationId);
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();

			return NtiWarningLetterMappers.ToServiceModel(dto, statuses);
		}

		public async Task StoreWarningLetter(NtiWarningLetterModel ntiWarningLetter, string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			await _ntiWarningLetterCachedService.SaveNtiWarningLetter(NtiWarningLetterMappers.ToDBModel(ntiWarningLetter), continuationId);
		}

		public async Task<NtiWarningLetterModel> GetNtiWarningLetterId(long wlId)
		{
			var dto = await _ntiWarningLetterCachedService.GetNtiWarningLetterAsync(wlId);
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();

			return NtiWarningLetterMappers.ToServiceModel(dto, statuses);
		}

		public async Task<NtiWarningLetterModel> PatchNtiWarningLetter(NtiWarningLetterModel patchWarningLetter)
		{
			patchWarningLetter.UpdatedAt = DateTime.Now;
			var dto = NtiWarningLetterMappers.ToDBModel(patchWarningLetter);
			var patchedDto = await _ntiWarningLetterCachedService.PatchNtiWarningLetterAsync(dto);
			var statuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();

			return NtiWarningLetterMappers.ToServiceModel(patchedDto, statuses);
		}
	}
}