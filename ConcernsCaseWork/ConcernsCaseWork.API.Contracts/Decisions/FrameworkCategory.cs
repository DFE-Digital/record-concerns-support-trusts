using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Decisions
{
	public enum FrameworkCategory
	{
		[Description("Enabling financial recovery")]
		EnablingFinancialRecovery = 1,
		[Description("Building financial capability")]
		BuildingFinancialCapability = 2,
		[Description("Facilitating transfer - financially agreed")]
		FacilitatingTransferFinanciallyAgreed = 3,
		[Description("Facilitating transfer - educationally triggered")]
		FacilitatingTransferEducationallyTriggered = 4,
		[Description("Emergency funding")]
		EmergencyFunding = 5
	}
}
