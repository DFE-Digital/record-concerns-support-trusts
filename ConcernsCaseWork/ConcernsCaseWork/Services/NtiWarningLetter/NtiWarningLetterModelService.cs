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

		public NtiWarningLetterModelService(INtiWarningLetterCachedService ntiWarningLetterCachedService)
		{
			_ntiWarningLetterCachedService = ntiWarningLetterCachedService;
		}

		public async Task<NtiWarningLetterModel> CreateNtiWarningLetter(NtiWarningLetterModel ntiWarningLetterModel)
		{
			var created = await _ntiWarningLetterCachedService.CreateNtiWarningLetterAsync(NtiWarningLetterMappers.ToDBModel(ntiWarningLetterModel));
			return NtiWarningLetterMappers.ToServiceModel(created);
		}

		public async Task<NtiWarningLetterModel> GetNtiWLById(long wlId)
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

		public async Task<ICollection<NtiWarningLetterModel>> GetNtiWLsForCase(long caseUrn)
		{
			var dtos = await _ntiWarningLetterCachedService.GetNtiWarningLettersForCaseAsync(caseUrn);
			return dtos?.Select(dto => NtiWarningLetterMappers.ToServiceModel(dto)).ToArray();
		}
	}
}