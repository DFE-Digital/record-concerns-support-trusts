using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Mvc;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TargetedTrustEngagement
{
	public class EditTargetedTrustEngagementModel : AbstractPageModel
	{
		[BindProperty(SupportsGet = true, Name = "urn")] 
		public int CaseUrn { get; set; }

		[BindProperty(Name = "activity-id")]
		public string ActivityId { get; set; }

		[BindProperty(Name = "sub-activity-id")]
		public RadioButtonsUiComponent SubActivityRadio { get; set; }

		public void OnGet()
        {
        }

		public void OnPost()
		{
			
		}
    }
}
