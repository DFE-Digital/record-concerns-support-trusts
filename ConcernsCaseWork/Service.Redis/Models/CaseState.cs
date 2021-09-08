using System;
using System.Collections.Generic;
using System.Numerics;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CaseState
	{
		public string TrustUkPrn { get; set; }
		public IDictionary<BigInteger, CaseStateWrapper> CasesDetails { get; } = new Dictionary<BigInteger, CaseStateWrapper>();
	}
}