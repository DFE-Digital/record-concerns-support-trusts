using Newtonsoft.Json;

namespace ConcernsCasework.Service.Trusts
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