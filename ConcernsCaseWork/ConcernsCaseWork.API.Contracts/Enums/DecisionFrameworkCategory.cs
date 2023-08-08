using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Enums
{
	public enum DecisionFrameworkCategory
	{
		[Description("EnablingFinancialRecovery")]
		EnablingFinancialRecovery = 1,

		[Description("BuildingFinancialCapacity")]
		BuildingFinancialCapacity = 2,

		[Description("FacilitatingTransferFinanciallyTriggered")]
		FacilitatingTransferFinanciallyTriggered = 3,

		[Description("FacilitatingTransferEducationallyTriggered")]
		FacilitatingTransferEducationallyTriggered = 4,
	}

}