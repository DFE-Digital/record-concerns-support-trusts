using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Used to model the rating labels
	/// </summary>
	public sealed class RatingLabelModel
	{
		public long Id { get; set; }
		
		public string Label { get; set; }

		public List<string> Names { get; set; }
	}
}