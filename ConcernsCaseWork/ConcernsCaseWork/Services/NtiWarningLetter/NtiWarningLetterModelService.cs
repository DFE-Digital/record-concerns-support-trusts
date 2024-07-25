using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using ConcernsCaseWork.Service.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.NtiWarningLetter
{
	public class NtiWarningLetterModelService : INtiWarningLetterModelService
	{
		private readonly INtiWarningLetterCachedService _ntiWarningLetterCachedService;
		private readonly ICasePermissionsService _casePermissionsService;

		public NtiWarningLetterModelService(
			INtiWarningLetterCachedService ntiWarningLetterCachedService, 
			ICasePermissionsService casePermissionsService)
		{
			_ntiWarningLetterCachedService = ntiWarningLetterCachedService;
			_casePermissionsService = casePermissionsService;
		}

		public async Task<NtiWarningLetterModel> CreateNtiWarningLetter(NtiWarningLetterModel ntiWarningLetterModel)
		{
			var created = await _ntiWarningLetterCachedService.CreateNtiWarningLetterAsync(NtiWarningLetterMappers.ToDBModel(ntiWarningLetterModel));

			return NtiWarningLetterMappers.ToServiceModel(created);
		}

		public async Task<IEnumerable<NtiWarningLetterModel>> GetNtiWarningLettersForCase(long caseUrn)
		{
			var dtos = await _ntiWarningLetterCachedService.GetNtiWarningLettersForCaseAsync(caseUrn);

			return dtos?.Select(dto => NtiWarningLetterMappers.ToServiceModel(dto)).ToArray();
		}

		public async Task<NtiWarningLetterModel> GetWarningLetter(string continuationId)
		{
			if (string.IsNullOrWhiteSpace(continuationId))
			{
				throw new ArgumentNullException(nameof(continuationId));
			}

			var dto = await _ntiWarningLetterCachedService.GetNtiWarningLetter(continuationId);

			return NtiWarningLetterMappers.ToServiceModel(dto);
		}

		public async Task<NtiWarningLetterModel> GetNtiWarningLetterViewModel(long caseId, long warningLetterId)
		{
			var dto = await _ntiWarningLetterCachedService.GetNtiWarningLetterAsync(warningLetterId);
			var permissionsResponse = await _casePermissionsService.GetCasePermissions(caseId);

			return NtiWarningLetterMappers.ToServiceModel(dto, permissionsResponse);
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

			return NtiWarningLetterMappers.ToServiceModel(dto);
		}

		public async Task<NtiWarningLetterModel> PatchNtiWarningLetter(NtiWarningLetterModel patchWarningLetter)
		{
			patchWarningLetter.UpdatedAt = DateTime.Now;
			var dto = NtiWarningLetterMappers.ToDBModel(patchWarningLetter);
			var patchedDto = await _ntiWarningLetterCachedService.PatchNtiWarningLetterAsync(dto);

			return NtiWarningLetterMappers.ToServiceModel(patchedDto);
		}

		public async Task DeleteNtiWarningLetter(long caseUrn, long wlId)
		{
			await _ntiWarningLetterCachedService.DeleteNtiWarningLetter(caseUrn, wlId);
		}
	}
}