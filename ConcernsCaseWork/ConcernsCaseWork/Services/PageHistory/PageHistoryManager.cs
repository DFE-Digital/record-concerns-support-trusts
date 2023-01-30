using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Services.PageHistory;

public static class PageHistoryManager
{
	public static IEnumerable<string> BuildPageHistory(string currentPage, IEnumerable<string> pageHistory)
	{
		var currentPageLCase = currentPage.ToLower();
		var updatedPageHistory = pageHistory.ToList();

		// Don't process requests which are api calls, error pages, or assets.
		if (!IsPageRequest(currentPageLCase)) return updatedPageHistory;
		
		// The logic here is that if we have visited page A, then gone to page B, and then to page A again (A --> B --> A) - we have effectively gone 'back' from 
		// page B to page A. Therefore we should remove the last item from page history, because we don't want any back button action to go to page B - we already
		// came from there. If we went back to page B, we would be stuck in a loop.
		// If we want to go back again, it should be to the page that was visited before page A (if any).
		if (HasPageGoneBack(currentPageLCase, updatedPageHistory))
		{
			updatedPageHistory.RemoveAt(updatedPageHistory.Count - 1);
		}
		
		// If we have refreshed the page, or hit a validation error, then the current page will be the same as the last page in history - ie a duplicate. 
		// We don't want to go back to that page, so we need to check for it so we don't include it in the page history more than once.
		if (!IsDuplicate(currentPageLCase, updatedPageHistory))
		{
			updatedPageHistory.Add(currentPageLCase);
		}
		
		return updatedPageHistory;
	}

	public static string GetPreviousPage(List<string> pageHistory) => pageHistory.Count <= 1 ? string.Empty : pageHistory[^2];
	
	private static bool IsPageRequest(string path) => !path.StartsWith("/v2/") && path != "/favicon.ico" && !path.StartsWith("/error");
	
	private static bool IsDuplicate(string path, IEnumerable<string> history) => history.LastOrDefault() == path;
	
	private static bool HasPageGoneBack(string path, List<string> history) => history.Count >=2 && history[^2] == path;
}