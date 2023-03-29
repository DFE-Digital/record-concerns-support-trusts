using ConcernsCaseWork.Models.CaseActions;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases.Create;

public interface ICreateCaseService
{
	Task<long> CreateNonConcernsCase(string userName,string trustUkPrn, string trustCompaniesHouseNumber);
	Task<long> CreateNonConcernsCase(string userName,string trustUkPrn, string trustCompaniesHouseNumber, SRMAModel srmaModel);
}