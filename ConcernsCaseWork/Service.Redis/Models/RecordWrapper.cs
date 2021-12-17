using Service.TRAMS.Records;
using System;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class RecordWrapper
	{
		public RecordDto RecordDto { get; set; }
	}
}