using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
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