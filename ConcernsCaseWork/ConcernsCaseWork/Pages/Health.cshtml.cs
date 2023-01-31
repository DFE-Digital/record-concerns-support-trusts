using ConcernsCaseWork.Attributes;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
namespace ConcernsCaseWork.Pages
{
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
