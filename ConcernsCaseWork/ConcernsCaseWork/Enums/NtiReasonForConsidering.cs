using System;
using System.ComponentModel;

namespace ConcernsCaseWork.Enums
{
	[Flags]
	public enum NtiReasonForConsidering
	{
		None = 0,
		
		[Description("Cash flow problems")]
		CashFlowProblems = 1,

		[Description("Cumulative deficit (actual)")]
		CumulativeDeficitActual = 1 << 1,

		[Description("Cumulative deficit (projected)")]
		CumulativeDeficitProjected = 1 << 2,

		[Description("Governance concerns")]
		GovernanceConcerns = 1 << 3,

		[Description("Non-compliance with academies financial/trust handbook")]
		NonComplianceWithAcademies = 1 << 4,

		[Description("Non-compliance with financial returns")]
		NonComplianceWithFinancialReturns = 1 << 5,

		[Description("Risk of insolvency")]
		RiskOfInsolvency = 1 << 6,

		[Description("Safeguarding")]
		Safeguarding = 1 << 7
	}
}
