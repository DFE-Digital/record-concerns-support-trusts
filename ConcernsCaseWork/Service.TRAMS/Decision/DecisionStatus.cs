using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Decision
{
	public enum DecisionStatus
	{
		[Description("In progress")]
		InProgress = 1,

		[Description("Closed")]
		Closed = 2,
	}
}
