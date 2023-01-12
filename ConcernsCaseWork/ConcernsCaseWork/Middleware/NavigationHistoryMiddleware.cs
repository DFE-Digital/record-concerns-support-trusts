using ConcernsCaseWork.Services.PageHistory;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Middleware;

public class NavigationHistoryMiddleware
{
	private readonly RequestDelegate _next;

	public NavigationHistoryMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext httpContext, IPageHistoryStorageHandler storageHandler)
	{
		var currentPage = ((string)httpContext.Request.Path).ToLower();
		var pageHistory = storageHandler.GetPageHistory(httpContext).ToArray();
			
		try
		{
			var updatedPageHistory = PageHistoryManager.BuildPageHistory(currentPage, pageHistory).ToArray();
			if (!updatedPageHistory.SequenceEqual(pageHistory))
			{
				storageHandler.SetPageHistory(httpContext, updatedPageHistory);
			}
		}
		catch
		{
			// ignored. If the session isn't available, not a lot we can do - but we don't want to stop processing
		}
		
		await _next(httpContext);
	}
}