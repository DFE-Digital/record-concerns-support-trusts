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

		[Description("Governance capability")]
		GovernanceCapability = 25,

		[Description("Non-compliance")]
		NonCompliance = 26,

		[Description("Actual and/or projected deficit")]
		ActualProjectedDeficit = 27,

		[Description("Actual and/or projected cash shortfall")]
		ActualProjectedCashShortfall = 28,

		[Description("Trust Closure")]
		TrustColure = 29,

		[Description("Financial management/ATH compliance")]
		FinancialManagementCompliance = 30,

		[Description("Late financial returns")]
		LateFinancialReturns = 31,

		[Description("Irregularity and/or self-reported fraud")]
		IrregularitySelfReportedFraud = 32
	}
}
