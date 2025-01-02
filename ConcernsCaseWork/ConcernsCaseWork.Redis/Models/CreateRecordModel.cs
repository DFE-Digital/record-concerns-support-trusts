using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Utils.Extensions;
using System;

namespace ConcernsCaseWork.Redis.Models
{
	[Serializable]
	public sealed class CreateRecordModel
	{
		public long CaseUrn { get; set; }
		
		public long TypeId { get; set; }

		public long RatingId { get; set; }

		public long StatusId { get; set; }

		public long MeansOfReferralId { get; set; }

		public string GetConcernTypeName()
		{
			return ((ConcernType?)TypeId)?.Description();
		}
		public bool IsClosed()
		{
			return StatusId == (long)ConcernStatus.Close;
		}
	}
}