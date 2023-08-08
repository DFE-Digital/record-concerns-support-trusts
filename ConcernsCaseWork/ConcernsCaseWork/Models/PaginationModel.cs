namespace ConcernsCaseWork.Models
{
	public class PaginationModel
	{
		public string Url { get; set; }

		public bool HasNext { get; set; }
		
		public bool HasPrevious { get; set; }

		public int TotalPages { get; set; }

		public int PageNumber { get; set; }

		public int? Next { get; set; }

		public int? Previous { get; set; }

		public int RecordCount { get; set; }

		/// <summary>
		/// The ID of the container that has the content to be changed
		/// This is for partial page reloads when pagination is invoked
		/// When we only want to refresh the content container, not the entire page
		/// </summary>
		public string? ContentContainerId { get; set; }

		/// <summary>
		/// Prefix so that we can have multiple pagination elements on screen
		/// Ensures we can uniquely identify the pagination for separate content containers
		/// </summary>
		public string ElementIdPrefix { get; set; }
	}
}
