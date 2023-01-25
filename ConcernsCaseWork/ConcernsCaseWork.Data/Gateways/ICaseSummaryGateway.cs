namespace ConcernsCaseWork.Data.Gateways;

public interface ICaseSummaryGateway
{
	Task<IList<ActiveCaseSummaryVm>> GetActiveCaseSummariesByOwner(string ownerId);
	Task<IList<ActiveCaseSummaryVm>> GetActiveCaseSummariesByTeamMember(string[] teamMemberIds);
	Task<IList<ClosedCaseSummaryVm>> GetClosedCaseSummariesByOwner(string ownerId); 
	Task<IList<ActiveCaseSummaryVm>> GetActiveCaseSummariesByTrust(string trustUkPrn); 
	Task<IList<ClosedCaseSummaryVm>> GetClosedCaseSummariesByTrust(string trustUkPrn); 
}