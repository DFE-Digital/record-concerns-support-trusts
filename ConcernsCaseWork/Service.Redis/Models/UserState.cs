using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class UserState
	{
		public UserState(string userName)
		{
			UserName = userName;
		}
		public string TrustUkPrn { get; set; }
		//public CreateCaseWizardModelDto CreateCaseWizardModel { get; set; }
		public CreateCaseModel CreateCaseModel { get; set; } = new CreateCaseModel();
		public IDictionary<long, CaseWrapper> CasesDetails { get; } = new ConcurrentDictionary<long, CaseWrapper>();
		public string UserName { get; private set; }
	}

	/*[Serializable]
	public class CreateCaseWizardModelDto
	{
		public int CaseType { get; set; }
	}*/
}