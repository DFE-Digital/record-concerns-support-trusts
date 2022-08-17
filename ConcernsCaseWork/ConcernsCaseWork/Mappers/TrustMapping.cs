using ConcernsCasework.Service.Trusts;
using System;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class TrustMapping
	{
		public static string FetchTrustName(TrustDetailsDto trustDetailsDto)
		{
			return trustDetailsDto?.GiasData?.GroupName ?? "-";
		}

		public static string FetchAcademies(TrustDetailsDto trustDetailsDto)
		{
			return string.Join(",", trustDetailsDto?.Establishments?.Select(e => e.EstablishmentName) ?? Array.Empty<string>());
		}
	}
}