using Newtonsoft.Json;

namespace Service.TRAMS.Trusts
{
	public sealed class CensusDto
	{
		[JsonProperty("numberOfPupils")]
		public string NumberOfPupils { get; }

		[JsonConstructor]
		public CensusDto(string numberOfPupils) =>
			(NumberOfPupils) =
			(numberOfPupils);
	}
}