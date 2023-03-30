using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Trusts
{
	public class EstablishmentTypeDto
	{
		[JsonProperty("name")]
		public virtual string Name { get; set; }
		
		[JsonProperty("code")]
		public virtual string Code { get; set; }
		
		[JsonConstructor]
		public EstablishmentTypeDto(string name, string code) => 
			(Name, Code) = 
			(name, code);
		
		protected EstablishmentTypeDto() { }
	}
}