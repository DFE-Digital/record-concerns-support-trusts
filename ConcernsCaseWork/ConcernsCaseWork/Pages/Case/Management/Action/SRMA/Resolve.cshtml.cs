﻿using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ResolvePageModel : AbstractPageModel
	{
		private readonly ILogger<ResolvePageModel> _logger;
		private readonly ISRMAService _srmaModelService;

		public SrmaCloseTextModel CloseTextModel { get; set; }

		public SRMAModel SRMAModel { get; set; }

		public ResolvePageModel(
			ISRMAService srmaModelService, ILogger<ResolvePageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::Action::SRMA::ResolvePageModel::OnGetAsync");
				(long caseUrn, long srmaId, string resolution) = GetRouteData();

				// TODO - get SRMA by case ID and SRMA ID
				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);

				if (SRMAModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}");
				}

				SetupPage(resolution);
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
			try
			{
				(long caseUrn, long srmaId, string resolution) = GetRouteData();

				SetupPage(resolution);

				SRMAStatus resolvedStatus;

				switch (resolution)
				{
					case SrmaConstants.ResolutionComplete:
						resolvedStatus = SRMAStatus.Complete;
						break;
					case SrmaConstants.ResolutionCancelled:
						resolvedStatus = SRMAStatus.Cancelled;
						break;
					case SrmaConstants.ResolutionDeclined:
						resolvedStatus = SRMAStatus.Declined;
						break;
					default:
						throw new Exception("resolution value is null or invalid to parse");
				}

				var srmaNotes = Request.Form["srma-notes"].ToString();
				if (!string.IsNullOrEmpty(srmaNotes))
				{
					if (srmaNotes.Length > 2000)
					{
						throw new Exception("Notes provided exceed maximum allowed length 2000 characters).");
					}
				}

				await _srmaModelService.SetNotes(srmaId, srmaNotes);
				await _srmaModelService.SetStatus(srmaId, resolvedStatus);
				await _srmaModelService.SetDateClosed(srmaId);

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

			var validResolutions = new List<string>() { SrmaConstants.ResolutionComplete, SrmaConstants.ResolutionDeclined, SrmaConstants.ResolutionCancelled };
			var resolutionValue = RouteData.Values["resolution"]?.ToString();

			if (string.IsNullOrEmpty(resolutionValue) || !validResolutions.Contains(resolutionValue))
			{
				throw new Exception("resolution value is null or invalid to parse");
			}

			return (caseUrn, srmaId, resolutionValue);
		}

		private void SetupPage(string resolution)
		{
			CloseTextModel = CaseActionsMapping.ToSrmaCloseText(resolution);
			ViewData[ViewDataConstants.Title] = CloseTextModel.Title;
		}
	}
}