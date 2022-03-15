using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class AddPageModel : AbstractPageModel
	{
		private readonly ILogger<AddPageModel> _logger;
		
		public AddPageModel(
			ILogger<AddPageModel> logger)
		{
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			_logger.LogInformation("Case::Action::SRMA::AddPageModel::OnGetAsync");
		}
	}
}