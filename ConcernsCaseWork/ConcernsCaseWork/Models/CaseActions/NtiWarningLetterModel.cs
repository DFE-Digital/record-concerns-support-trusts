using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiWarningLetterModel : CaseActionModel
	{
		public int Status { get; set; }
		public ICollection<int> Reasons { get; set; }
		public string Notes { get; set; }

		public DateTime SentDate { get; set; }
	}
}
