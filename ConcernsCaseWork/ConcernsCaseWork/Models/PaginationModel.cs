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
	}
}
