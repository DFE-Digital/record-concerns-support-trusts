using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Tokens;

namespace ConcernsCaseWork.TagHelpers;

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
			output.TagName ="span";
			output.Attributes.SetAttribute("class", "govuk-body");
			output.Content.SetContent(Text);
		}
	}
}