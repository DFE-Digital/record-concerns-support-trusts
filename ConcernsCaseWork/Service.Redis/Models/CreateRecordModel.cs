using System;
using System.Collections.Generic;

namespace Service.Redis.Models
{
	[Serializable]
	public sealed class CreateRecordModel
	{
		public string Reason { get; set; }

		public string Type { get; set; } = string.Empty;

		public string SubType { get; set; } = string.Empty;

		public string TypeDisplay
		{
			get
			{
				var separator = string.IsNullOrEmpty(SubType) ? string.Empty : ":";
				return $"{SubType}{separator} {SubType ?? string.Empty}";
			}
		}

		public string Name { get; set; }

		public long Urn { get; set; }

		public bool Checked { get; set; }

		public Tuple<int, IList<string>> RagRating { get; set; }

		public IList<string> RagRatingCss { get; set; }
	}
}