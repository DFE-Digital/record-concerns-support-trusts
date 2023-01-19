using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ConcernsCaseWork.TagHelpers;

public static class TagOutputBuilder
{
	public static void BuildEmptyTextTag(this TagHelperOutput output)
	{
		output.TagName = "span";
		output.Attributes.SetAttribute("class", "govuk-tag ragtag ragtag__grey");
		output.Content.SetContent("Empty");
	}
}