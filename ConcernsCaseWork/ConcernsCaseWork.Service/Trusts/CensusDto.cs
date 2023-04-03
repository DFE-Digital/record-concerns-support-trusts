using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class CensusDto
	{
		[JsonProperty("numberOfPupils")]
		public virtual string NumberOfPupils { get; set; }

		[JsonConstructor]
		public CensusDto(string numberOfPupils) =>
			(NumberOfPupils) =
			(numberOfPupils);

		protected CensusDto() { }
	}
}