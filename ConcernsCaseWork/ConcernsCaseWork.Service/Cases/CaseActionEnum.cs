using System.ComponentModel;

namespace ConcernsCaseWork.Service.Cases
{
	public enum CaseActionEnum
	{
		[Description("Decision")]
		Decision,
		DfESupport,
		FinancialForecast,
		[Description("Financial plan")]
		FinancialPlan,
		FinancialReturns,
		FinancialSupport,
		ForcedTermination,
		[Description("NTI: Notice to improve")]
		Nti,
		RecoveryPlan,
		[Description("SRMA (School Resource Management Adviser)")]
		Srma,
		[Description("TFF (trust financial forecast)")]
		TrustFinancialForecast,
		[Description("NTI: Under consideration")]
		NtiUnderConsideration,
		[Description("NTI: Warning letter")]
		NtiWarningLetter,
		[Description("Targeted Trust Engagement")]
		TargetedTrustEngagement
	}
}