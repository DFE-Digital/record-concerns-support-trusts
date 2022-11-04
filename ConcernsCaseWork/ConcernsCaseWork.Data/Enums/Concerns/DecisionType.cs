using System.ComponentModel;

namespace ConcernsCaseWork.Data.Enums.Concerns
{
    public enum DecisionType
    {
	    [Description("Notice to Improve (NTI)")]
	    NoticeToImprove = 1,

	    [Description("Section 128 (S128)")]
	    Section128 = 2,

	    [Description("Qualified Floating Charge (QFC)")]
	    QualifiedFloatingCharge = 3,

	    [Description("Non-repayable financial support")]
	    NonRepayableFinancialSupport = 4,

	    [Description("Repayable financial support")]
	    RepayableFinancialSupport = 5,

	    [Description("Short-term cash advance")]
	    ShortTermCashAdvance = 6,

	    [Description("Write-off recoverable funding")]
	    WriteOffRecoverableFunding = 7,

	    [Description("Other financial support")]
	    OtherFinancialSupport = 8,

	    [Description("Other financial support")]
	    EstimatesFundingOrPupilNumberAdjustment = 9,

	    [Description("ESFA approval to spend or write-off")]
	    EsfaApproval = 10,

	    [Description("Freedom of Information exemptions (FOI) ")]
	    FreedomOfInformationExemptions = 11
    }
}
