using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ConcernsCaseWork.TagHelpers
{
	[HtmlTargetElement("govuk-radios-item", TagStructure = TagStructure.NormalOrSelfClosing)]
	public class RadiosItemTagHelper : TagHelper
	{
		[HtmlAttributeName("id")]
		public string Id { get; set; }

		[HtmlAttributeName("name")]
		public string Name { get; set; }

		[HtmlAttributeName("label")]
		public string Label { get; set; }

		[HtmlAttributeName("value")]
		public string Value { get; set; }

		[HtmlAttributeName("hint")]
		public string Hint { get; set; }

		[HtmlAttributeName("asp-for")]
		public ModelExpression For { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "div";
			output.Attributes.SetAttribute("class", "govuk-radios__item");

			output.TagMode = TagMode.StartTagAndEndTag;

			output.Content.SetHtmlContent(
				$@"<input class=""govuk-radios__input"" id=""{Id}"" name=""{Name}"" type=""radio"" value=""{Value}"">
					<label class=""govuk-label govuk-radios__label"" for=""other-vulnerability"">
						{Label}
					</label>
					<div class=""govuk-hint"">
						{Hint}
					</div>
				"); 
		}
	}
}
