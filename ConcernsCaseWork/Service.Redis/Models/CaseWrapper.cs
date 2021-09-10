using Service.TRAMS.Cases;
using Service.TRAMS.Records;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CaseWrapper
	{
		public CaseDto CaseDto { get; set; }
		public IDictionary<BigInteger, RecordDto> Records { get; set; } = new Dictionary<BigInteger, RecordDto>();
	}
}