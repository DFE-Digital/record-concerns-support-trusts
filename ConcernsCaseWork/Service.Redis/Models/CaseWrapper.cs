using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CaseWrapper
	{
		public CaseDto CaseDto { get; set; }
		public IDictionary<BigInteger, RecordWrapper> Records { get; set; } = new Dictionary<BigInteger, RecordWrapper>();
	}
}