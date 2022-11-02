using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RatingModel
	{
		public string Name { get; set; }
		
		public long Id { get; set; }

		public bool Checked { get; set; }
		
		public Tuple<int, IList<string>> RagRating { get; set; }

		public IList<string> RagRatingCss { get; set; }
	}
}