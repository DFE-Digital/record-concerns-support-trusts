namespace ConcernsCaseWork.Service.Trusts
{
	public interface ITrustSearchService
	{
		Task<TrustSearchResponseDto> GetTrustsBySearchCriteria(TrustSearch trustSearch);
	}
}