using System;
using System.Collections.Generic;
using System.Numerics;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class UserState
	{
		public string TrustUkPrn { get; set; }
		public IDictionary<BigInteger, CaseWrapper> CasesDetails { get; } = new Dictionary<BigInteger, CaseWrapper>();
	}
}