using JetBrains.Annotations;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Tokens;

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
public class MultiLineTextTagHelper : TagHelper
{
	public string Contents { get; set; }
	
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		if (Contents.IsNullOrEmpty())
		{
			output.BuildEmptyTextTag();
		}
		else
		{
			output.TagName = "span";
			output.Attributes.SetAttribute("class", "dfe-text-area-display");
			output.Content.SetContent(Contents);
		}
	}
}

public class SingleLineTextTagHelper : TagHelper
{
	public string Text { get; set; }
	
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{		
		if (Text.IsNullOrEmpty())
		{
			output.BuildEmptyTextTag();
		}
		else
		{
			output.Content.SetContent(Text);
		}
	}
}

public static class TagOutputBuilder
{
	public static void BuildEmptyTextTag(this TagHelperOutput output)
	{
		output.TagName = "span";
		output.Attributes.SetAttribute("class", "govuk-tag ragtag ragtag__grey");
		output.Content.SetContent("Empty");
	}
}

