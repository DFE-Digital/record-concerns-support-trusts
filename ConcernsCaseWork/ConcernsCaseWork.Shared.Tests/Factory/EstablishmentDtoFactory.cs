using Service.TRAMS.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class EstablishmentDtoFactory
	{
		public static List<EstablishmentDto> CreateListEstablishmentDto()
		{
			return new List<EstablishmentDto>
			{
				new EstablishmentDto("establishment-urn", "establishment-name", "establishment-ukprn")
			};
		}
	}
}