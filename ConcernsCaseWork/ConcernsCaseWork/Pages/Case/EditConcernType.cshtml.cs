using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	public class EditConcernTypePageModel : PageModel
	{
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<EditConcernTypePageModel> _logger;

		public IDictionary<string, IList<string>> TypesDictionary { get; private set; }
		
		public EditConcernTypePageModel(ITypeModelService typeModelService, ILogger<EditConcernTypePageModel> logger)
		{
			_typeModelService = typeModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			_logger.LogInformation("EditConcernTypePageModel::OnGetAsync");
			
			TypesDictionary = await _typeModelService.GetTypes();

			return Page();
		}
		
		public void OnPostEditConcernType(string type, string subType)
		{
			_logger.LogInformation("EditConcernTypePageModel::OnPostEditConcernType");
		}
	}
}