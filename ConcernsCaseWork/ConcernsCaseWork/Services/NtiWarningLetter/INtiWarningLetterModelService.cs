using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.NtiWarningLetter
{
	public interface INtiWarningLetterModelService
	{
		Task<NtiWarningLetterModel> CreateNtiWarningLetter(NtiWarningLetterModel ntiWarningLetterModel);
		Task<ICollection<NtiWarningLetterModel>> GetNtiWLsForCase(long caseUrn);
		Task<NtiWarningLetterModel> GetWarningLetterFromCache(Guid continuationId);
		Task StoreWarningLetterToCache(NtiWarningLetterModel ntiWarningLetter, Guid continuationId);
	}
}
