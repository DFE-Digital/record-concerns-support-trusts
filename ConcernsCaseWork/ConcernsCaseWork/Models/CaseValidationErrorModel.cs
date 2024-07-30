using ConcernsCaseWork.API.Contracts.Case;

namespace ConcernsCaseWork.Models
{
	public class CaseValidationErrorModel
	{
		public CaseValidationError Type { get; set; }
		public string Error { get; set; }
	}
}
