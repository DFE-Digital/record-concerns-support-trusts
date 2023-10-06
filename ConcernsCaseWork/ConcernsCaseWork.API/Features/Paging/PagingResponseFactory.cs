using ConcernsCaseWork.API.Contracts.Common;
using Microsoft.AspNetCore.Http.Extensions;

namespace ConcernsCaseWork.API.Features.Paging
{
	public static class PagingResponseFactory
	{
		public static PagingResponse Create(int page, int recordsPerPage, int recordCount, HttpRequest request)
		{
			var totalItemsSeen = page * recordsPerPage;
			int totalPages = (int)Math.Ceiling((double)recordCount / recordsPerPage);

			var pagingResponse = new PagingResponse
			{
				RecordCount = recordCount,
				Page = page,
				HasNext = totalItemsSeen < recordCount,
				HasPrevious = totalItemsSeen - recordsPerPage > 0,
				TotalPages = totalPages
			};

			if (totalItemsSeen >= recordCount) return pagingResponse;

			var queryAttributes = request.Query
				.Where(q => q.Key != nameof(page) && q.Key != "count")
				.Select(q => new KeyValuePair<string, string>(q.Key, q.Value));

			var queryBuilder = new QueryBuilder(queryAttributes)
			{
				{"page", $"{page + 1}"},
				{"count", $"{recordsPerPage}"}
			};

			pagingResponse.NextPageUrl = $"{request.Path}{queryBuilder}";

			return pagingResponse;
		}
	}
}