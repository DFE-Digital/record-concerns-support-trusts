using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.NtiWarningLetter
{
	public interface INtiWarningLetterModelService
	{
		Task<NtiWarningLetterModel> CreateNtiWarningLetter(NtiWarningLetterModel ntiWarningLetterModel);
		Task<ICollection<NtiWarningLetterModel>> GetNtiWarningLettersForCase(long caseUrn);
		Task<NtiWarningLetterModel> GetWarningLetter(string continuationId);
		Task StoreWarningLetter(NtiWarningLetterModel ntiWarningLetter, string continuationId);
		Task<NtiWarningLetterModel> GetWarningLetter(long warningLetterId);
	}
}
