using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CaseWrapper
	{
		public CaseDto CaseDto { get; set; }
		public IDictionary<long, RecordWrapper> Records { get; set; } = new Dictionary<long, RecordWrapper>();
	}
}