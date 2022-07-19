using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public interface INtiWarningLetterStatusesService
	{
		Task<ICollection<NtiWarningLetterStatusDto>> GetAllStatusesAsync();
	}
}
