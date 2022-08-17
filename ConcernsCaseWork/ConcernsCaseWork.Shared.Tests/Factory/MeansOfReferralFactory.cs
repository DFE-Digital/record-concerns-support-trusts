using ConcernsCaseWork.Models;
using ConcernsCasework.Service.MeansOfReferral;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class MeansOfReferralFactory
	{
		public static List<MeansOfReferralDto> BuildListMeansOfReferralDto()
		{
			return new List<MeansOfReferralDto>
			{
				new MeansOfReferralDto("Internal", "Some description 1", 1),
				new MeansOfReferralDto("External", "Some description 2", 2)
			};
		}

		public static List<MeansOfReferralModel> BuildListMeansOfReferralModels()
		{
			return new List<MeansOfReferralModel>
			{
				new MeansOfReferralModel{Name = "Internal", Description = "Some description 1", Urn = 1},
				new MeansOfReferralModel{Name = "External", Description = "Some description 2", Urn = 2},
			};
		}
	}
}