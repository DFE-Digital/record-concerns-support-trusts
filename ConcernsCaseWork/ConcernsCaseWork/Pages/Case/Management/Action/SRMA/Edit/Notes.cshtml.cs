using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditNotesPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaModelService;
		private readonly ILogger<EditNotesPageModel> _logger;

		public SRMAModel SRMAModel { get; set; }
		public int NotesMaxLength => 2000;

		public EditNotesPageModel(ISRMAService srmaModelService, ILogger<EditNotesPageModel> logger)
		{
			_srmaModelService = srmaModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::Action::SRMA::EditNotesPageModel::OnGetAsync");

				(long caseUrn, long srmaId) = GetRouteData();

				SRMAModel = await _srmaModelService.GetSRMAById(srmaId);
				
				if (SRMAModel.IsClosed)
				{
					return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}/closed");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditNotesPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}

			return Page();
		}

		public async Task<ActionResult> OnPostAsync()
		{
			long caseUrn = 0;
			long srmaId = 0;

			try
			{
				_logger.LogInformation("Case::Action::SRMA::EditNotesPageModel::OnPostAsync");

				(caseUrn, srmaId) = GetRouteData();
				var srmaNotes = Request.Form["srma-notes"].ToString();

				await _srmaModelService.SetNotes(srmaId, srmaNotes);
				return Redirect($"/case/{caseUrn}/management/action/srma/{srmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::SRMA::EditNotesPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}

		private (long caseUrn, long srmaId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["caseUrn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var srmaIdValue = RouteData.Values["srmaId"];
			if (srmaIdValue == null || !long.TryParse(srmaIdValue.ToString(), out long srmaId) || srmaId == 0)
				throw new Exception("srmaId is null or invalid to parse");

			return (caseUrn, srmaId);
		}
	}
}