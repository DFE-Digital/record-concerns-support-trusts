namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustSearchResponseDto
	{
		public IList<TrustSearchDto> Trusts { get; set; }
		public int NumberOfMatches { get; set; }
	}
}
