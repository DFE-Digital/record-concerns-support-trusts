using ConcernsCaseWork.API.Contracts.Concerns;

namespace ConcernsCaseWork.API.Contracts.Case
{
    public class CaseFilterParameters
    {
		public List<KeyValuePair<int, string>> Statuses { get; set; }
	}
}
