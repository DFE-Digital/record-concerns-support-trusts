using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditSRMAReasonOfferedPageModel : AbstractPageModel
	{
		private readonly ISRMAService srmaService;
		private readonly ILogger<EditSRMAReasonOfferedPageModel> _logger;

		public SRMAModel SRMA { get; set; }
		public IEnumerable<RadioItem> Reasons { get; private set; }

		public EditSRMAReasonOfferedPageModel(ISRMAService srmaService, ILogger<EditSRMAReasonOfferedPageModel> logger)
		{
			this.srmaService = srmaService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::EditSRMAReasonOfferedPageModel::OnGetAsync");

				var validationResponse = ValidateInputsForGet();
				if (validationResponse.validationErrors.Count() > 0)
				{
					TempData["SRMA.Message"] = validationResponse.validationErrors;
				}
				else
				{
					SRMA = await srmaService.GetSRMAById(validationResponse.srmaId);
					Reasons = GetReasons();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditSRMAReasonOfferedPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<ActionResult> OnPostAsync(string url)
		{
			try
			{
				_logger.LogInformation("Case::EditSRMAReasonOfferedPageModel::OnPostAsync");

				var validationResponse = ValidateInputsForPost();

				if (validationResponse.validationErrors.Count() > 0)
				{
					TempData["SRMA.Message"] = validationResponse.validationErrors;
					return Page();
				}

				await srmaService.SetReason(validationResponse.srmaId, validationResponse.reason);
				return Redirect($"/case/{validationResponse.caseId}/management/action/srma/{validationResponse.srmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditSRMAReasonOfferedPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private (List<string> validationErrors, long srmaId, long caseId) ValidateInputsForGet()
		{
			var validationErrors = new List<string>();

			var caseUrnStr = Convert.ToString(RouteData.Values["caseUrn"]);
			if (!long.TryParse(caseUrnStr, out long caseUrn))
			{
				validationErrors.Add("Invalid case Id");
			}

			var srmaIdStr = Convert.ToString(RouteData.Values["srmaId"]);
			if (!long.TryParse(srmaIdStr, out long srmaId))
			{
				validationErrors.Add("SRMA Id not found");
			}

			return (validationErrors, srmaId, caseUrn);
		}

		private (List<string> validationErrors, long srmaId, SRMAReasonOffered reason, long caseId) ValidateInputsForPost()
		{
			var validationErrors = new List<string>();
			SRMAReasonOffered srmaReason = SRMAReasonOffered.Unknown;

			var caseUrnStr = Convert.ToString(RouteData.Values["caseUrn"]);
			if (!long.TryParse(caseUrnStr, out long caseUrn))
			{
				validationErrors.Add("Invalid case Id");
			}

			var srmaIdStr = Convert.ToString(RouteData.Values["srmaId"]);
			if (!long.TryParse(srmaIdStr, out long srmaId))
			{
				validationErrors.Add("SRMA Id not found");
			}

			var reason = Convert.ToString(Request.Form["reason"]);
			if (string.IsNullOrEmpty(reason))
			{
				validationErrors.Add("A reason not selected");
			}
			else if (!Enum.TryParse<SRMAReasonOffered>(reason, out srmaReason))
			{
				validationErrors.Add("Invalid reason");
			}

			return (validationErrors, srmaId, srmaReason, caseUrn);
		}

		private IEnumerable<RadioItem> GetReasons()
		{
			var reasons = (SRMAReasonOffered[])Enum.GetValues(typeof(SRMAReasonOffered));
			return reasons.Where(r => r != SRMAReasonOffered.Unknown)
						   .Select(r => new RadioItem
						   {
							   Id = r.ToString(),
							   Text = EnumHelper.GetEnumDescription(r),
							   IsChecked = r == SRMA.Reason
						   });
		}
	}
}