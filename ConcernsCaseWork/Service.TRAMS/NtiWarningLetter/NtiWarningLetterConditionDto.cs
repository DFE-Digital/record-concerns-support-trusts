﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.NtiWarningLetter
{
	public class NtiWarningLetterConditionDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		[JsonProperty("conditionType")]
		public NtiWarningLetterConditionTypeDto Type { get; set; }
	}
}
