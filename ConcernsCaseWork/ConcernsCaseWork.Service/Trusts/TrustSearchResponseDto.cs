namespace ConcernsCaseWork.Service.Trusts
{
	public class TrustSearchResponseDto
	{
		public virtual IList<TrustSearchDto> Trusts { get; set; }
		public virtual int NumberOfMatches { get; set; }
	}
}
