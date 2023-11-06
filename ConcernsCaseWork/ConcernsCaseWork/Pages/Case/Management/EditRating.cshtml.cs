using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Ratings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditRatingPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditRatingPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent RiskToTrust { get; set; }

		public EditRatingPageModel(
			ICaseModelService caseModelService, 
			ILogger<EditRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				var caseModel = await _caseModelService.GetCaseByUrn(CaseUrn);

				LoadPage(caseModel);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
		
		public async Task<ActionResult> OnPostEditRiskRating()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					LoadPage();
					return Page();
				}

				var patchCaseModel = new PatchCaseModel
				{
					Urn = CaseUrn,
					UpdatedAt = DateTimeOffset.Now,
					RatingId = (long)RiskToTrust.SelectedId
				};
					
				await _caseModelService.PatchCaseRating(patchCaseModel);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPage(CaseModel model)
		{
			LoadPage();
			RiskToTrust.SelectedId = (int)model.RatingId;
		}
		
		private void LoadPage()
		{
			RiskToTrust = CaseComponentBuilder.BuildRiskToTrust(nameof(RiskToTrust), RiskToTrust?.SelectedId);
			RiskToTrust.Heading = "";
		}
	}
}