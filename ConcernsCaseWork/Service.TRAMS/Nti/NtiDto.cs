using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.Nti
{
	public class NtiDto
	{
		public long Id { get; set; }
		public long CaseUrn { get; set; }
		public ICollection<NtiReasonDto> Reasons { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
		public string Notes { get; set; }
	}
}