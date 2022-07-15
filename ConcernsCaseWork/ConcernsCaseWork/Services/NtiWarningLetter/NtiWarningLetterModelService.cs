using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.NtiWarningLetter;
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

		public async Task<ICollection<NtiWarningLetterModel>> GetNtiWLsForCase(long caseUrn)
		{
			var dtos = await _ntiWarningLetterCachedService.GetNtiWarningLettersForCaseAsync(caseUrn);
			return dtos?.Select(dto => NtiWarningLetterMappers.ToServiceModel(dto)).ToArray();
		}
	}
}