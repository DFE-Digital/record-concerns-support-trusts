using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.NtiWarningLetter
{
	public class NtiWarningLetterDto
	{
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public string Notes { get; set; }
		public DateTime? ClosedAt { get; set; }
		public NtiWarningLetterStatusDto Status { get; set; }
		public ICollection<NtiWarningLetterReasonDto> Reasons { get; set; }
		public ICollection<NtiWarningLetterConditionDto> Conditions { get; set; }
		public DateTime? SentDate { get; set; }

	}
}
