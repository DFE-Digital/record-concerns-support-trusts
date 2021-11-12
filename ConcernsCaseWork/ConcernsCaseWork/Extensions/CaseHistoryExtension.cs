using Service.TRAMS.Cases;

namespace ConcernsCaseWork.Extensions
{
	public static class CaseHistoryExtension
	{
		public static string ToDisplay(this CaseHistoryEnum me)
		{
			return me switch
			{
				CaseHistoryEnum.Comment => "Comment",
				CaseHistoryEnum.Concern => "Concern",
				CaseHistoryEnum.Financial => "Financial",
				CaseHistoryEnum.ForcedTerminationOfFa => "Forced termination of FA",
				CaseHistoryEnum.Fnti => "FNTI",
				CaseHistoryEnum.Investigation => "Investigation",
				CaseHistoryEnum.Letter => "Letter",
				CaseHistoryEnum.LinkedCases => "Linked case(s)",
				CaseHistoryEnum.Outcome => "Outcome",
				CaseHistoryEnum.PraSupport => "PRA Support",
				CaseHistoryEnum.RecoveryPlan => "Recovery plan",
				CaseHistoryEnum.Srma => "SRMA",
				CaseHistoryEnum.Tff => "TFF",
				CaseHistoryEnum.Whistleblower => "Whistleblower",
				_ => "n/f"
			};
		}
	}
}