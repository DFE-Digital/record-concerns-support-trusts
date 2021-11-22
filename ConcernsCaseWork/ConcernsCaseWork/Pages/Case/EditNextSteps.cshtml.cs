using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditNextStepsPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditNextStepsPageModel> _logger;

		public CaseModel CaseModel { get; private set; }

		public EditNextStepsPageModel(ICaseModelService caseModelService, ILogger<EditNextStepsPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;

			try
			{
				_logger.LogInformation("Case::EditNextStepsPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditNextStepsPageModel::CaseUrn is null or invalid to parse");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditNextStepsPageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}


			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}

		public async Task<ActionResult> OnPostEditNextSteps(string url)
		{
			long caseUrn = 0;

			try
			{
				_logger.LogInformation("Case::EditNextStepsPageModel::OnPostEditCaseAim");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditNextStepsPageModel::CaseUrn is null or invalid to parse");
				}

				var nextSteps = Request.Form["next-steps"];

				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					NextSteps = nextSteps
				};

				await _caseModelService.PatchNextSteps(patchCaseModel);

				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditNextStepsPageModel::OnPostEditNextSteps::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn);
		}


		private async Task<ActionResult> LoadPage(string url, long caseUrn)
		{
			if (caseUrn == 0) return Page();

			CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			CaseModel.PreviousUrl = url;

			return Page();
		}
	}
}
