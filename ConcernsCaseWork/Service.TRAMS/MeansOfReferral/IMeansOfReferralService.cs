using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.MeansOfReferral
{
	public interface IMeansOfReferralService
	{
		Task<IList<MeansOfReferralDto>> GetMeansOfReferrals();
	}
}