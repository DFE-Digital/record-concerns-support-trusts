using Service.TRAMS.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class TrustDtoFactory
	{
		public static List<TrustDto> CreateListTrustDto()
		{
			return new List<TrustDto>
			{
				new TrustDto(
					"trust-ukprn", 
					"trust-urn", 
					"trust-groupname", 
					"trust-companieshousenumber", 
					EstablishmentDtoFactory.CreateListEstablishmentDto())
			};
		}
	}
}