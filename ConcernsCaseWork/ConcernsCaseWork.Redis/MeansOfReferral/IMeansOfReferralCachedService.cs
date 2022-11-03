using ConcernsCaseWork.Service.MeansOfReferral;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.MeansOfReferral
{
	public interface IMeansOfReferralCachedService
	{
		Task<IList<MeansOfReferralDto>> GetMeansOfReferralsAsync();
	}
}