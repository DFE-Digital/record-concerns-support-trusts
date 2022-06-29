using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.Nti
{
	public class NtiReasonDto
	{
		public long Id { get; set; }
		public string Name { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
