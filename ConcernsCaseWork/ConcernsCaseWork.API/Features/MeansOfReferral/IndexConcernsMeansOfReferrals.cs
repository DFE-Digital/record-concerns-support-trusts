﻿using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.MeansOfReferral
{
	public interface IIndexConcernsMeansOfReferrals
	{
		public IList<ConcernsMeansOfReferralResponse> Execute();
	}

	public class IndexConcernsMeansOfReferrals : IIndexConcernsMeansOfReferrals
	{
		private readonly IConcernsMeansOfReferralGateway _concernsMeansOfReferralGateway;

		public IndexConcernsMeansOfReferrals(IConcernsMeansOfReferralGateway concernsMeansOfReferralGateway)
		{
			_concernsMeansOfReferralGateway = concernsMeansOfReferralGateway;
		}
		public IList<ConcernsMeansOfReferralResponse> Execute()
		{
			var types = _concernsMeansOfReferralGateway.GetMeansOfReferrals();
			return types.Select(ConcernsMeansOfReferralResponseFactory.Create).ToList();
		}
	}
}