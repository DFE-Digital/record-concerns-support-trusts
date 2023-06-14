namespace ConcernsCaseWork.API.ResponseModels
{
    public class PagingResponse
    {
		/// <summary>
		/// The current page we are on
		/// </summary>
        public int Page { get; set; }

		/// <summary>
		/// This is the total record count
		/// </summary>
        public int RecordCount { get; set; }
        public string NextPageUrl { get; set; }
		public bool HasNext { get; set; }

		public bool HasPrevious { get; set; }
    }
}