using Newtonsoft.Json;

namespace ConcernsCasework.Service.Nti
{
	public class NtiConditionDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		[JsonProperty("conditionType")]
		public NtiConditionTypeDto Type { get; set; }

	}
}
