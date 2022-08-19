using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.Nti
{
	public class NtiConditionDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		[JsonProperty("conditionType")]
		public NtiConditionTypeDto Type { get; set; }

	}
}
