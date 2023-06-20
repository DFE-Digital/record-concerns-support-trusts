namespace ConcernsCaseWork.Data.Gateways;

public interface ICaseSummaryGateway
{
	Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByOwner(GetCaseSummariesByOwnerParameters parameters);
	Task<IList<ActiveCaseSummaryVm>> GetActiveCaseSummariesByTeamMembers(string[] teamMemberIds);
	Task<IList<ClosedCaseSummaryVm>> GetClosedCaseSummariesByOwner(string ownerId);
	Task<(IList<ActiveCaseSummaryVm>, int)> GetActiveCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters); 
	Task<(IList<ClosedCaseSummaryVm>, int)> GetClosedCaseSummariesByTrust(GetCaseSummariesByTrustParameters parameters); 
}