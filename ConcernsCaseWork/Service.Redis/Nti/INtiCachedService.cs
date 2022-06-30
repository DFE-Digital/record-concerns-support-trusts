using Service.TRAMS.Nti;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.Nti
{
	public interface INtiCachedService
	{
		Task<NtiDto> CreateNti(NtiDto nti);
		Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn);
		Task<NtiDto> GetNTIUnderConsiderationById(long underConsiderationId);
	}
}
