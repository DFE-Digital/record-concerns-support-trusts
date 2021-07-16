using Newtonsoft.Json;

namespace Service.TRAMS.Models
{
	public sealed class NameAndCodeDto
	{
		public string Code { get; }
		public string Name { get; }
		
		[JsonConstructor]
		public NameAndCodeDto(string code , string name)
		{
			Code = code;
			Name = name;
		}
	}
}