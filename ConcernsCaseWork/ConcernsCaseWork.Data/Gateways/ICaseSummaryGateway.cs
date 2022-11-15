namespace ConcernsCaseWork.Data.Gateways;

public interface ICaseSummaryGateway
{
	Task<IList<CaseSummary>> GetActiveCaseSummaries(string ownerId); 
}