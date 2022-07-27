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
		Task<NtiWarningLetterModel> GetWarningLetterFromCache(string continuationId);
		Task StoreWarningLetterToCache(NtiWarningLetterModel ntiWarningLetter, string continuationId);
	}
}
