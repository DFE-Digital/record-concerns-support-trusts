using Service.TRAMS.Cases;
using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public interface ICaseCachedService
	{
		Task<CaseDto> PostCase(CreateCaseDto createCaseDto, string caseworker);
	}
}