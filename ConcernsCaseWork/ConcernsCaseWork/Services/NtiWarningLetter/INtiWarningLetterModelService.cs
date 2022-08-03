using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.NtiWarningLetter
{
	public interface INtiWarningLetterModelService
	{
		Task<NtiWarningLetterModel> CreateNtiWarningLetter(NtiWarningLetterModel ntiWarningLetterModel);
		Task<NtiWarningLetterModel> GetNtiWLById(long wlId);
		Task<ICollection<NtiWarningLetterModel>> GetNtiWLsForCase(long caseUrn);
		Task<NtiWarningLetterModel> PatchNtiWarningLetter(NtiWarningLetterModel patchWarningLetter);
	}
}
