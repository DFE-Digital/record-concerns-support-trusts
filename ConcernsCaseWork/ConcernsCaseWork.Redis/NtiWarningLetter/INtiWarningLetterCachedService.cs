using ConcernsCaseWork.Service.NtiWarningLetter;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.NtiWarningLetter
{
	public interface INtiWarningLetterCachedService
	{
		Task SaveNtiWarningLetter(NtiWarningLetterDto ntiWarningLetter, string continuationId);
		Task<NtiWarningLetterDto> GetNtiWarningLetter(string continuationId);
	}
}
