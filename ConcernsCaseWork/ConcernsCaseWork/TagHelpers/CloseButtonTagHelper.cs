using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ConcernsCaseWork.TagHelpers;

public class CloseButtonTagHelper : TagHelper
{
	public string Url { get; set; }
	public string Label { get; set; }
	public string TestId { get; set; }
	
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{		
		output.TagName = "a";
		output.Attributes.SetAttribute("href", Url);
		output.Attributes.SetAttribute("class", "govuk-button govuk-!-margin-top-6");
		output.Attributes.SetAttribute("data-prevent-double-click", "true");
		output.Attributes.SetAttribute("data-module", "govuk-button");
		output.Attributes.SetAttribute("role", "button");
		output.Attributes.SetAttribute("data-testid", TestId);
		output.Content.SetContent(Label);
	}
}