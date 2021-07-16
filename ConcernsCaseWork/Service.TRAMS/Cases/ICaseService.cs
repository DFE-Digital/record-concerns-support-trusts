using Service.TRAMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseService
	{
		// TODO - for example only...
		Task<IEnumerable<CaseDto>> GetCasesByCaseworker(string caseworker);
	}
}