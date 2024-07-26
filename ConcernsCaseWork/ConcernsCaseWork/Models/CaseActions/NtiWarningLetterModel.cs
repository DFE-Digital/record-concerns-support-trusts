using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class NtiWarningLetterModel : CaseActionModel
	{
		public NtiWarningLetterStatus? Status { get; set; }
		public ICollection<NtiWarningLetterReason> Reasons { get; set; } = new List<NtiWarningLetterReason>();
		public ICollection<NtiWarningLetterConditionModel> Conditions { get; set; } = new List<NtiWarningLetterConditionModel>();
		public string Notes { get; set; }
		public DateTime? SentDate { get; set; }
		public NtiWarningLetterStatus? ClosedStatusId { get; set; }

		public bool CanBeEdited()
		{
			return !ClosedAt.HasValue;
		}
	}
}