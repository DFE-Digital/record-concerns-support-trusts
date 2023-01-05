using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ConcernsCaseWork.Services.PageHistory;

public interface IPageHistoryStorageHandler
{
	public void SetPageHistory(HttpContext httpContext,IEnumerable<string> pageHistory);
	public IEnumerable<string> GetPageHistory(HttpContext httpContext);
}