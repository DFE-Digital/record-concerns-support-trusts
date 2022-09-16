using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public interface INtiService
	{
		Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn);
		Task<NtiDto> CreateNtiAsync(NtiDto newNtiWarningLetter);
		Task<NtiDto> GetNtiAsync(long ntiWarningLetterId);
		Task<NtiDto> PatchNtiAsync(NtiDto ntiWarningLetter);
	}
}
