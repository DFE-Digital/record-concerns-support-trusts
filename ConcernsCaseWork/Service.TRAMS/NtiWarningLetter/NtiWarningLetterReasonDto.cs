using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.NtiWarningLetter
{
	public class NtiWarningLetterReasonDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
