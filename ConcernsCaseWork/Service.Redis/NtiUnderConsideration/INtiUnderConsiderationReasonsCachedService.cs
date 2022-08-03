using Service.TRAMS.NtiUnderConsideration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Redis.NtiUnderConsideration
{
	public interface INtiUnderConsiderationReasonsCachedService
	{
		Task ClearData();
		Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons();
	}
}
