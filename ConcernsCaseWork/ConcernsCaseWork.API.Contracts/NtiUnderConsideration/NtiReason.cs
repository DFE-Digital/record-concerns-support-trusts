using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.NtiUnderConsideration
{
	public enum NtiUnderConsiderationReason
	{
		[Description("Cash flow problems")]
		CashFlowProblems = 1,
		[Description("Cumulative deficit (actual)")]
		CumulativeDeficitActual = 2,
		[Description("Cumulative deficit (projected)")]
		CumulativeDeficitProjected = 3,
		[Description("Governance concerns")]
		GovernanceConcerns = 4,
		[Description("Non-Compliance with Academies Financial/Trust Handbook")]
		NonComplianceWithAcademiesFinancialTrustHandbook = 5,
		[Description("Non-Compliance with financial returns")]
		NonComplianceWithFinancialReturns = 6,
		[Description("Risk of insolvency")]
		RiskOfInsolvency = 7,
		[Description("Safeguarding")]
		Safeguarding = 8
	}
}
