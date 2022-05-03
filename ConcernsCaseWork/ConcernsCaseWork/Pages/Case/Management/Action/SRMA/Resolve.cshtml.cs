using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Srma
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ResolvePageModel : AbstractPageModel
	{
		private readonly ILogger<ResolvePageModel> _logger;
		private readonly ISRMAService _srmaModelService;

		private const string ResolutionComplete = "complete";
		private const string ResolutionCanceled = "cancel";
		private const string ResolutionDeclined = "decline";

		public string ConfirmText { get; set; }

		public SRMAModel SRMAModel { get; set; }

		public ResolvePageModel(
			ISRMAService srmaModelService, ILogger<ResolvePageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			long srmaId = 0;
			string resoultion = "";

			try
			{
				_logger.LogInformation("Case::Action::SRMA::ResolvePageModel::OnGetAsync");
				(caseUrn, srmaId, resoultion) = GetRouteData();

				// TODO - get SRMA by case ID and SRMA ID
				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);

				switch (resoultion)
				{
					case ResolutionCanceled:
						ConfirmText = "Confirm SRMA was canceled";
						break;
					case ResolutionDeclined:
						ConfirmText = "Confirm SRMA was declined by trust";
						break;
					case ResolutionComplete:
						ConfirmText = "Confirm SRMA is complete";
						break;
					default:
						throw new Exception("resolution value is null or invalid to parse");
						break;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::ResolvePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			long caseUrn = 0;
			long srmaId = 0;
			string resoultion = "";

			try
			{
				(caseUrn, srmaId, resoultion) = GetRouteData();

				SRMAStatus resolvedStatus;

				switch (resoultion)
				{
					case ResolutionComplete:
						resolvedStatus = SRMAStatus.Complete;
						break;
					case ResolutionCanceled:
						resolvedStatus = SRMAStatus.Canceled;
						break;
					case ResolutionDeclined:
						resolvedStatus = SRMAStatus.Declined;
						break;
					default:
						throw new Exception("resolution value is null or invalid to parse");
						break;
				}

				var srmaNotes = Request.Form["srma-notes"].ToString();
				await _srmaModelService.SetNotes(srmaId, srmaNotes);
				await _srmaModelService.SetStatus(srmaId, resolvedStatus);
				await _srmaModelService.SetDateClosed(srmaId, DateTime.Now);

				return Redirect($"/case/{caseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::SRMA::ResolvePageModel::OnPostAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
				return Page();
			}
		}

		private (long caseUrn, long srmaId, string resolution) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var srmaIdValue = RouteData.Values["srmaId"];
			if (srmaIdValue == null || !long.TryParse(srmaIdValue.ToString(), out long srmaId) || srmaId == 0)
				throw new Exception("srmaId is null or invalid to parse");

			var validResolutions = new List<string>() { ResolutionComplete, ResolutionDeclined, ResolutionCanceled };
			var resolutionValue = RouteData.Values["resolution"].ToString();

			if (string.IsNullOrEmpty(resolutionValue) || !validResolutions.Contains(resolutionValue) )
			{
				throw new Exception("resolution value is null or invalid to parse");
			}

			return (caseUrn, srmaId, resolutionValue);
		}
	}
}