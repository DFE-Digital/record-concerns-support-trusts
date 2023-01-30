using System;

namespace ConcernsCaseWork.Redis.Models
{
	[Serializable]
	public sealed class UserState
	{
		public UserState(string userName)
		{
			UserName = userName;
		}
		public string TrustUkPrn { get; set; }
		public CreateCaseModel CreateCaseModel { get; set; } = new ();
		public string UserName { get; private set; }
	}
}