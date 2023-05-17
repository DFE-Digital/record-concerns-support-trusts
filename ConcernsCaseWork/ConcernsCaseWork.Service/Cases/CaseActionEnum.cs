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
		[Description("TFF (Trust Financial Forecast)")]
		TrustFinancialForecast,
		[Description("NTI: Under consideration")]
		NtiUnderConsideration,
		[Description("NTI: Warning letter")]
		NtiWarningLetter
	}
}