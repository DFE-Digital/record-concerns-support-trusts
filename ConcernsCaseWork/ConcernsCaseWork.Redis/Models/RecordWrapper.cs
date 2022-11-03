using ConcernsCaseWork.Service.Records;
using System;

namespace ConcernsCaseWork.Redis.Models
{
	[Serializable]
	public sealed class RecordWrapper
	{
		public RecordDto RecordDto { get; set; }
	}
}