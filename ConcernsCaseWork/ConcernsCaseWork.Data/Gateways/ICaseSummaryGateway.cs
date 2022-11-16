namespace ConcernsCaseWork.Data.Gateways;

public interface ICaseSummaryGateway
{
	Task<IList<CaseSummaryVm>> GetActiveCaseSummaries(string ownerId); 
}