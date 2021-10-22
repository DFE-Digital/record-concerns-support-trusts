using Newtonsoft.Json;

namespace Service.TRAMS.Trusts
{
	public sealed class EstablishmentTypeDto
	{
		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("code")]
		public string Code { get; }
		
		[JsonConstructor]
		public EstablishmentTypeDto(string name, string code) => 
			(Name, Code) = 
			(name, code);
	}
}