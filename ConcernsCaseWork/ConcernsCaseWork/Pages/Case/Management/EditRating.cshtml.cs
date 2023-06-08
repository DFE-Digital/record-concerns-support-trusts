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
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<EditRatingPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent RiskToTrust { get; set; }

		public EditRatingPageModel(ICaseModelService caseModelService, 
			IRatingModelService ratingModelService, 
			ILogger<EditRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_ratingModelService = ratingModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGet()
		{
			_logger.LogMethodEntered();

			try
			{
				var caseModel = await _caseModelService.GetCaseByUrn(CaseUrn);

				await LoadPage(caseModel);
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
					await LoadPage();
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

		private async Task LoadPage(CaseModel model)
		{
			await LoadPage();
			RiskToTrust.SelectedId = (int)model.RatingId;
		}
		
		private async Task LoadPage()
		{
			var ratingsModel = await _ratingModelService.GetRatingsModel();

			RiskToTrust = CaseComponentBuilder.BuildRiskToTrust(nameof(RiskToTrust), ratingsModel, RiskToTrust?.SelectedId);
			RiskToTrust.Heading = "";
		}
	}
}