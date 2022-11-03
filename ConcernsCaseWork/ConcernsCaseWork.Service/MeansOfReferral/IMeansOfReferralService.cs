namespace ConcernsCaseWork.Service.MeansOfReferral
{
	public interface IMeansOfReferralService
	{
		Task<IList<MeansOfReferralDto>> GetMeansOfReferrals();
	}
}