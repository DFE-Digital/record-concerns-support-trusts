using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ConcernsCaseWork.TagHelpers;

public class ClosedLabelTagHelper : TagHelper
{
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{		
		output.TagName = "span";
		output.Attributes.SetAttribute("class", "govuk-tag ragtag ragtag__grey govuk-!-margin-bottom-6");
		output.Content.SetContent("Closed");
	}
}