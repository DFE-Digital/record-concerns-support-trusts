using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.NtiWarningLetter
{
	public interface INtiWarningLetterModelService
	{
		Task<NtiWarningLetterModel> CreateNtiWarningLetter(NtiWarningLetterModel ntiWarningLetterModel);
		Task<NtiWarningLetterModel> GetNtiWarningLetterId(long wlId);
		Task<NtiWarningLetterModel> PatchNtiWarningLetter(NtiWarningLetterModel patchWarningLetter);
		Task<IEnumerable<NtiWarningLetterModel>> GetNtiWarningLettersForCase(long caseUrn);
		Task<NtiWarningLetterModel> GetWarningLetter(string continuationId);
		Task StoreWarningLetter(NtiWarningLetterModel ntiWarningLetter, string continuationId);
	}
}
