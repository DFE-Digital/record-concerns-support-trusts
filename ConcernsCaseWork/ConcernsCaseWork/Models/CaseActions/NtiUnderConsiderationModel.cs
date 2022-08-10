﻿using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiUnderConsiderationModel : CaseActionModel
	{
		public ICollection<NtiReasonForConsideringModel> NtiReasonsForConsidering { get; set; }
		public string Notes { get; set; }
		public int? ClosedStatusId { get; set; }
		public string ClosedStatusName { get; set; }
	}
}