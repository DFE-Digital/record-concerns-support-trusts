using Service.TRAMS.MeansOfReferral;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.MeansOfReferral
{
	public interface IMeansOfReferralCachedService
	{
		Task<IList<MeansOfReferralDto>> GetMeansOfReferralsAsync();
	}
}