using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service.TRAMS.Models
{
	/// <summary>
	/// TODO missing mappings from the real response.
	/// </summary>
	public sealed class TrustDto
	{
		public List<EstablishmentDto> Establishments { get; }

		[JsonConstructor]
		public TrustDto(List<EstablishmentDto> establishments)
		{
			Establishments = establishments;
		}
	}
}