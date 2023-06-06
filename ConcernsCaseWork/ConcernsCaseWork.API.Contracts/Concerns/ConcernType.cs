using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Concerns
{
	public enum ConcernType
	{
		[Description("Financial: Deficit")]
		FinancialDeficit = 3,

		[Description("Financial: Projected deficit")]
		FinancialProjectedDeficit = 4,

		[Description("Force majeure")]
		ForceMajeure = 7,

		[Description("Governance and compliance: Governance")]
		Governance = 8,

		[Description("Financial: Viability")]
		FinancialViability = 20,

		[Description("Irregularity: Irregularity")]
		Irregularity = 21,

		[Description("Irregularity: Suspected fraud")]
		IrregularitySuspectedFraud = 22,

		[Description("Governance and compliance: Compliance")]
		Compliance = 23,

		Safeguarding = 24
	}
}
