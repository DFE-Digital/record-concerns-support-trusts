using ConcernsCaseWork.Models.CaseActions;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases.Create;

public interface ICreateCaseService
{
	Task<long> CreateNonConcernsCase(string userName);
	Task<long> CreateNonConcernsCase(string userName, SRMAModel srmaModel);
}