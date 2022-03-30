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

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditSRMAStatusPageModel : AbstractPageModel
	{
		private readonly ILogger<EditCurrentStatusPageModel> _logger;

		public SRMAModel SRMA { get; set; }

		public IEnumerable<RadioItem> SRMAStatuses => getStatuses();

		public EditSRMAStatusPageModel(ICaseModelService caseModelService, ILogger<EditCurrentStatusPageModel> logger)
		{
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::EditSRMAStatusPageModel::OnGetAsync");


			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCurrentStatusPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<ActionResult> OnPostEditCurrentStatus(string url)
		{
			try
			{
				_logger.LogInformation("Case::EditSRMAStatusPageModel::OnPostEditCurrentStatus");

			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCurrentStatusPageModel::OnPostEditCurrentStatus::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private IEnumerable<RadioItem> getStatuses()
		{
			var statuses = (SRMAStatus[])Enum.GetValues(typeof(SRMAStatus));
			return statuses.Select(s => new RadioItem { Id = s.ToString(), Text = EnumHelper.GetEnumDescription(s) });
		}
	}
}