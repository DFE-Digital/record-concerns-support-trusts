using ConcernsCaseWork.Extensions;
using NUnit.Framework;
using ConcernsCaseWork.Service.Cases;

namespace ConcernsCaseWork.Tests.Extensions
{
	[Parallelizable(ParallelScope.All)]
	public class CaseHistoryExtensionTests
	{
		[TestCase(CaseHistoryEnum.Case, "Case")]
		[TestCase(CaseHistoryEnum.Comment, "Comment")]
		[TestCase(CaseHistoryEnum.Concern, "Concern")]
		[TestCase(CaseHistoryEnum.Financial, "Financial")]
		[TestCase(CaseHistoryEnum.ForcedTerminationOfFa, "Forced termination of FA")]
		[TestCase(CaseHistoryEnum.Fnti, "FNTI")]
		[TestCase(CaseHistoryEnum.Investigation, "Investigation")]
		[TestCase(CaseHistoryEnum.Letter, "Letter")]
		[TestCase(CaseHistoryEnum.LinkedCases, "Linked case(s)")]
		[TestCase(CaseHistoryEnum.Outcome, "Outcome")]
		[TestCase(CaseHistoryEnum.PraSupport, "PRA Support")]
		[TestCase(CaseHistoryEnum.RecoveryPlan, "Recovery plan")]
		[TestCase(CaseHistoryEnum.Srma, "SRMA")]
		[TestCase(CaseHistoryEnum.Tff, "TFF")]
		[TestCase(CaseHistoryEnum.Whistleblower, "Whistleblower")]
		[TestCase(CaseHistoryEnum.NotFound, "n/f")]
		public void WhenToDisplay_ReturnsDisplayString(CaseHistoryEnum caseHistoryEnum, string expected)
		{
			Assert.That(caseHistoryEnum.ToDisplay(), Is.EqualTo(expected));
		}
	}
}