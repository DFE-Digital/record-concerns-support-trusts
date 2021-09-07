using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models.Redis
{
	[Serializable]
	public sealed class CaseStateModel
	{
		public string TrustUkPrn { get; set; }
		public Dictionary<string, CaseStateWrapperModel> CasesDetails { get; } = new Dictionary<string, CaseStateWrapperModel>();
	}
}