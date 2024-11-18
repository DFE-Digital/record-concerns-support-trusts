using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace ConcernsCaseWork.TagHelpers;

public class MultiLineTextTagHelper : TagHelper
{
	public string Contents { get; set; }
	
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		if (string.IsNullOrEmpty(Contents))
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