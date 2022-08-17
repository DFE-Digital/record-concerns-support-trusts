using ConcernsCasework.Service.MeansOfReferral;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.MeansOfReferral
{
	public interface IMeansOfReferralCachedService
	{
		Task<IList<MeansOfReferralDto>> GetMeansOfReferralsAsync();
	}
}