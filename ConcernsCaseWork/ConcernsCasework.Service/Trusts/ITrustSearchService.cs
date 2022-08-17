namespace ConcernsCasework.Service.Trusts
{
	public interface ITrustSearchService
	{
		Task<IList<TrustSearchDto>> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}