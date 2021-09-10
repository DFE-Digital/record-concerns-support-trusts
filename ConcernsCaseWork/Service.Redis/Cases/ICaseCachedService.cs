using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public interface ICaseCachedService
	{
		Task<IList<CaseDto>> GetCasesByCaseworker(string caseworker, string statusUrn = "Live");
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto);
		Task<Boolean> IsCasePrimary(string caseworker);
	}
}