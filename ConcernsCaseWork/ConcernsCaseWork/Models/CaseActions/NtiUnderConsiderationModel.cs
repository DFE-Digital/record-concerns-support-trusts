using ConcernsCaseWork.Enums;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiUnderConsiderationModel : CaseActionModel
	{
		public long CaseUrn { get; set; }
		NtiReasonForConsidering NtiReasonForConsidering { get; set; }
		public string Notes { get; set; }


	}
}
