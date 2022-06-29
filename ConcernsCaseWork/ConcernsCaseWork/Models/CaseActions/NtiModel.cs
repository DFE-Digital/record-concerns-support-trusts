﻿using ConcernsCaseWork.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiModel : CaseActionModel
	{
		public ICollection<NtiReasonForConsideringModel> NtiReasonsForConsidering { get; set; }
		public string Notes { get; set; }

	}
}
