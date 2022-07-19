using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiUnderConsideration
{
	public interface INtiUnderConsiderationReasonsService
	{
		Task<ICollection<NtiUnderConsiderationReasonDto>> GetAllReasons();
	}
}
