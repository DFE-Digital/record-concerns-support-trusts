using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.NtiWarningLetter
{
	public interface INtiWarningLetterConditionsService
	{
		Task<ICollection<NtiWarningLetterConditionDto>> GetAllConditionsAsync();
	}
}
