using Newtonsoft.Json;

namespace ConcernsCasework.Service.NtiWarningLetter
{
	public class NtiWarningLetterConditionDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		[JsonProperty("conditionType")]
		public NtiWarningLetterConditionTypeDto Type { get; set; }
	}
}
