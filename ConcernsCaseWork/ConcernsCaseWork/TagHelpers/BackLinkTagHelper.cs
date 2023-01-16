using JetBrains.Annotations;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ConcernsCaseWork.TagHelpers;

public class BackLinkTagHelper : TagHelper
{
	public string Url { get; set; }
	public string Label { get; set; } = "Back";
	
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "a";
		output.Attributes.Add("id", "back-link-event");
		output.Attributes.SetAttribute("href", Url);
		output.Attributes.SetAttribute("class", "govuk-back-link");
		output.Content.SetContent(Label);
	}
}