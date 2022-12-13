using ConcernsCaseWork.Service.Base;

namespace ConcernsCaseWork.Service.Cases
{
	public interface ICaseSearchService
	{
		Task<IList<CaseDto>> GetCasesByPageSearch(PageSearch pageSearch);
	}
}