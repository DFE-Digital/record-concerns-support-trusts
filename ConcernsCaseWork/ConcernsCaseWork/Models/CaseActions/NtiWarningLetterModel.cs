using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiWarningLetterModel : CaseActionModel
	{
		public NtiWarningLetterStatus? Status { get; set; }
		public ICollection<NtiWarningLetterReason> Reasons { get; set; }
		public ICollection<NtiWarningLetterConditionModel> Conditions { get; set; }
		public string Notes { get; set; }
		public DateTime? SentDate { get; set; }
		public NtiWarningLetterStatus? ClosedStatusId { get; set; }

		public bool CanBeEdited()
		{
			return !ClosedAt.HasValue;
		}
	}
}