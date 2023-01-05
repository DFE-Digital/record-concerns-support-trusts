using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;

namespace ConcernsCaseWork.Pages;

public class AccessibilityPageModel : AbstractPageModel
{
	public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);
}