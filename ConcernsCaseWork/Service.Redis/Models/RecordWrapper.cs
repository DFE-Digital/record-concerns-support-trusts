using ConcernsCasework.Service.Records;
using System;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class RecordWrapper
	{
		public RecordDto RecordDto { get; set; }
	}
}