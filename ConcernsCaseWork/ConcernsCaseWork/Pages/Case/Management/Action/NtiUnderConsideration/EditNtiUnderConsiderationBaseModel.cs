using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	public class EditNtiUnderConsiderationBaseModel : AbstractPageModel
	{
		protected IEnumerable<RadioItem> GetReasons(NtiUnderConsiderationModel ntiModel = null)
		{
			var reasonValues = Enum.GetValues<NtiUnderConsiderationReason>().ToList();

			return reasonValues.Select(r => new RadioItem
			{
				Id = Convert.ToString((int)r),
				Text = r.Description(),
				IsChecked = ntiModel?.NtiReasonsForConsidering?.Any(ntiR => ntiR == r) == true,
			});
		}

		protected TextAreaUiComponent BuildNotesComponent(string name, string contents = "")
		=> new("nti-notes", name, "Notes (optional)")
		{
			HintText = "Case owners can record any information they want that feels relevant to the action.",
			Text = new ValidateableString()
			{
				MaxLength = NtiConstants.MaxNotesLength,
				StringContents = contents,
				DisplayName = "Notes"
			}
		};
	}
}
