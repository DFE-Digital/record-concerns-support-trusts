using System;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CreateRecordModel
	{
		public long CaseUrn { get; set; }
		
		public long TypeId { get; set; }

		public string Type { get; set; } = string.Empty;

		public string SubType { get; set; } = string.Empty;

		public string TypeDisplay
		{
			get
			{
				var separator = string.IsNullOrEmpty(SubType) ? string.Empty : ":";
				return $"{Type}{separator} {SubType ?? string.Empty}";
			}
		}

		public long RatingId { get; set; }

		public string RatingName { get; set; }

		public Tuple<int, IList<string>> RagRating { get; set; }

		public IList<string> RagRatingCss { get; set; }
		
		public long MeansOfReferralId { get; set; }
	}
}