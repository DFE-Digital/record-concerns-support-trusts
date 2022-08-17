namespace ConcernsCasework.Service.MeansOfReferral
{
	public interface IMeansOfReferralService
	{
		Task<IList<MeansOfReferralDto>> GetMeansOfReferrals();
	}
}