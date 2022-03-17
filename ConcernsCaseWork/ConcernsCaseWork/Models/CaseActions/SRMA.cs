using ConcernsCaseWork.Enums;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class SRMA : CaseAction
	{
		public DateTime	DateOffered { get; set; }
		public SRMAStatus Status { get; set; }
		public string Notes { get; set; } 
	}
}
