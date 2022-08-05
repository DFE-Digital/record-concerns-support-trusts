using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.MeansOfReferral
{
	public interface IMeansOfReferralModelService
	{
		Task<IList<MeansOfReferralModel>> GetMeansOfReferrals();
	}
}