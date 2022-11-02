using ConcernsCaseWork.Service.NtiWarningLetter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterCachedService
	{
		Task<NtiWarningLetterDto> CreateNtiWarningLetterAsync(NtiWarningLetterDto newNtiWarningLetter);
		Task<ICollection<NtiWarningLetterDto>> GetNtiWarningLettersForCaseAsync(long caseUrn);
		Task<NtiWarningLetterDto> GetNtiWarningLetterAsync(long ntiWarningLetterId);
		Task<NtiWarningLetterDto> PatchNtiWarningLetterAsync(NtiWarningLetterDto ntiWarningLetter);
		Task SaveNtiWarningLetter(NtiWarningLetterDto ntiWarningLetter, string continuationId);
		Task<NtiWarningLetterDto> GetNtiWarningLetter(string continuationId);
	}
}
