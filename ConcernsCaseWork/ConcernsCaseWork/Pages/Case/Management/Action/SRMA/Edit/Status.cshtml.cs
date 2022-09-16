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
	public class EditSRMAStatusPageModel : AbstractPageModel
	{
		private readonly ISRMAService srmaService;
		private readonly ILogger<EditCurrentStatusPageModel> _logger;

		public SRMAModel SRMA { get; set; }
		public IEnumerable<RadioItem> SRMAStatuses { get; private set; }

		public EditSRMAStatusPageModel(ISRMAService srmaService, ILogger<EditCurrentStatusPageModel> logger)
		{
			this.srmaService = srmaService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::EditSRMAStatusPageModel::OnGetAsync");

				var validationResponse = ValidateInputsForGet();
				if (validationResponse.validationErrors.Count() > 0)
				{
					TempData["SRMA.Message"] = validationResponse.validationErrors;
				}
				else
				{
					SRMA = await srmaService.GetSRMAById(validationResponse.srmaId);
					SRMAStatuses = getStatuses();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCurrentStatusPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<ActionResult> OnPostAsync(string url)
		{
			try
			{
				_logger.LogInformation("Case::EditSRMAStatusPageModel::OnPostEditCurrentStatus");

				var validationResponse = ValidateInputsForPost();

				if (validationResponse.validationErrors.Count() > 0)
				{
					TempData["SRMA.Message"] = validationResponse.validationErrors;
					return Page();
				}

				await srmaService.SetStatus(validationResponse.srmaId, validationResponse.status);
				return Redirect($"/case/{validationResponse.caseId}/management/action/srma/{validationResponse.srmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCurrentStatusPageModel::OnPostEditCurrentStatus::Exception - {Message}", ex.Message);
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

		private (List<string> validationErrors, long srmaId, SRMAStatus status, long caseId) ValidateInputsForPost()
		{
			var validationErrors = new List<string>();
			SRMAStatus srmaStatus = SRMAStatus.Unknown;

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

			var status = Convert.ToString(Request.Form["status"]);
			if (string.IsNullOrEmpty(status))
			{
				validationErrors.Add("SRMA status not selected");
			}
			else if (!Enum.TryParse<SRMAStatus>(status, out srmaStatus))
			{
				validationErrors.Add("Invalid SRMA status");
			}

			return (validationErrors, srmaId, srmaStatus, caseUrn);
		}

		private IEnumerable<RadioItem> getStatuses()
		{
			var statuses = (SRMAStatus[])Enum.GetValues(typeof(SRMAStatus));
			return statuses.Where(s => s != SRMAStatus.Unknown && s != SRMAStatus.Declined && s != SRMAStatus.Canceled && s != SRMAStatus.Complete)
						   .Select(s => new RadioItem
						   {
							   Id = s.ToString(),
							   Text = EnumHelper.GetEnumDescription(s),
							   IsChecked = s == SRMA.Status
						   });
		}
	}
}