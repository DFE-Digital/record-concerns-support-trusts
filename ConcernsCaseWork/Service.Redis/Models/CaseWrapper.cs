using Service.TRAMS.Cases;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CaseWrapper
	{
		public CaseDto CaseDto { get; set; }
		public IList<CaseHistoryDto> CasesHistoryDto { get; set; } = new List<CaseHistoryDto>();
		public IDictionary<long, RecordWrapper> Records { get; set; } = new ConcurrentDictionary<long, RecordWrapper>();
	}
}