using Service.TRAMS.Models;
using System;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CasesStateData
	{
		public string TrustUkPrn { get; set; }
		public Dictionary<string, CaseDto> CaseDetails { get; set; } = new Dictionary<string, CaseDto>();
	}
}