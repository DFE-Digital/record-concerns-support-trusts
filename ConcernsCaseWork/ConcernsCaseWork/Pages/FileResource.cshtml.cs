using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConcernsCaseWork.Pages
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class FileResource : PageModel
	{
		public IActionResult OnGetDownloadRiskManagementPdf()
		{
			Response.Headers.Add("content-disposition", "inline; filename=Risk_Management_Framework.pdf");
			
			return File("/files/Risk_Management_Framework.pdf", "application/octet-stream", 
				"Risk_Management_Framework.pdf");
		}
	}
}