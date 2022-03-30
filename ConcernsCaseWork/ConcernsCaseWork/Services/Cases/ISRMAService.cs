using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Models.CaseActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ISRMAService
	{
		public Task SaveSRMA(SRMAModel srma);
		public Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn);
		public Task<SRMAModel> GetSRMAById(long srmaId);
		public Task SetStatus(long srmaId, SRMAStatus status);
	}
}