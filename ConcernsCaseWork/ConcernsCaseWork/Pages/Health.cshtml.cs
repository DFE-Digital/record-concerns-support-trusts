using ConcernsCaseWork.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;


namespace ConcernsCaseWork.Pages
{
	[ResponseCache(NoStore = true, Duration = 0)]
	public class HealthModel : PageModel
	{
		public string BuildGuid { get; set; }

		public void OnGet()
		{
			var assembly = Assembly.GetEntryAssembly();
			this.BuildGuid = assembly.GetCustomAttribute<BuildGuidAttribute>().BuildGuid;
		}
	}
}
