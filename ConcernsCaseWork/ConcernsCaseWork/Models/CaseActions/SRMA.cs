using ConcernsCaseWork.Enums;
using System;

namespace ConcernsCaseWork.Models.CaseActions
{
	public class SRMA : CaseAction
	{
		public long Id { get; set; }
		public DateTime	DateOffered { get; set; }
		public SRMAStatus Status { get; set; }
		public string Notes { get; set; } 
	}
}
