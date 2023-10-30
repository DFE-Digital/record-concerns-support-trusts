using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Concerns
{
	public enum ConcernType
	{
		[Description("Deficit")]
		FinancialDeficit = 3,

		[Description("Projected deficit")]
		FinancialProjectedDeficit = 4,

		[Description("Force majeure")]
		ForceMajeure = 7,

		[Description("Financial governance")]
		FinancialGovernance = 8,

		[Description("Viability")]
		FinancialViability = 20,

		[Description("Irregularity")]
		Irregularity = 21,

		[Description("Suspected fraud")]
		IrregularitySuspectedFraud = 22,

		[Description("Financial compliance")]
		Compliance = 23,

		[Description("Safeguarding non-compliance")]
		Safeguarding = 24,

		[Description("Governance")]
		Governance = 25,

		[Description("Non-compliance")]
		NonCompliance = 26
	}
}
