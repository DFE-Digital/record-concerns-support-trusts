using System;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CreateRecordModel
	{
		public long TypeUrn { get; set; }

		public string Type { get; set; } = string.Empty;

		public string SubType { get; set; } = string.Empty;

		public string Reason
		{
			get
			{
				var separator = string.IsNullOrEmpty(SubType) ? string.Empty : ":";
				return $"{Type}{separator} {SubType ?? string.Empty}";
			}
		}

		public long RatingUrn { get; set; }

		public string RatingName { get; set; }

		public Tuple<int, IList<string>> RagRating { get; set; }

		public IList<string> RagRatingCss { get; set; }
	}
}