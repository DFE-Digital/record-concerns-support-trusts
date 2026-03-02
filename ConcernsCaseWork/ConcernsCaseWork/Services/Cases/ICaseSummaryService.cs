using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Service.Cases;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public interface ICaseSummaryService
{
	Task<CaseSearchParametersDto> GetCaseSearchCriterias();
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetCaseSummariesByFilter(
		Region[] regions = null,
		string[] caseOwners = null,
		string[] caseTeamLeaders = null,
		CaseStatus[] statuses = null,
		int? page = 1);
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker, int? page);
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCaseSummariesForUsersTeam(string caseworker, int? page);
	Task<CaseSummaryGroupModel<ClosedCaseSummaryModel>> GetClosedCaseSummariesByCaseworker(string caseworker, int? page);
	Task<CaseSummaryGroupModel<ActiveCaseSummaryModel>> GetActiveCaseSummariesByTrust(string trustUkPrn, int? page);
	Task<CaseSummaryGroupModel<ClosedCaseSummaryModel>> GetClosedCaseSummariesByTrust(string trustUkPrn, int? page);
}