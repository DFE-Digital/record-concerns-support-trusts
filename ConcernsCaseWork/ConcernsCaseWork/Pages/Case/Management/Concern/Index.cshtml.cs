using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.MeansOfReferral;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly IRecordModelService _recordModelService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<IndexPageModel> _logger;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent ConcernType { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent MeansOfReferral { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent ConcernRiskRating { get; set; }

		public IndexPageModel(ICaseModelService caseModelService,
			IRecordModelService recordModelService,
			ITrustModelService trustModelService,
			IRatingModelService ratingModelService,
			ILogger<IndexPageModel> logger)
		{
			_recordModelService = recordModelService;
			_ratingModelService = ratingModelService;
			_trustModelService = trustModelService;
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();


				await LoadPage();

			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}

				var typeId = (ConcernType)(ConcernType.SelectedSubId.HasValue ? ConcernType.SelectedSubId : ConcernType.SelectedId);

				var createRecordModel = new CreateRecordModel
				{
					CaseUrn = CaseUrn,
					TypeId = (long)typeId,
					Type = "",
					SubType = "",
					RatingId = ConcernRiskRating.SelectedId.Value,
					MeansOfReferralId = MeansOfReferral.SelectedId.Value
				};
				
				// Post record
				await _recordModelService.PostRecordByCaseUrn(createRecordModel);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}
			
			return await LoadPage();
		}
		
		private async Task<ActionResult> LoadPage()
		{
			// Get Case
			var caseModel = await _caseModelService.GetCaseByUrn(CaseUrn);
				
			CreateRecordsModel = await _recordModelService.GetCreateRecordsModelByCaseUrn(CaseUrn);
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseModel.TrustUkPrn);
			var ratingsModel = await _ratingModelService.GetRatingsModel();

			MeansOfReferral = CaseComponentBuilder.BuildMeansOfReferral(nameof(MeansOfReferral), MeansOfReferral?.SelectedId);
			ConcernRiskRating = CaseComponentBuilder.BuildConcernRiskRating(nameof(ConcernRiskRating), ratingsModel, ConcernRiskRating?.SelectedId);
			ConcernType = CaseComponentBuilder.BuildConcernType(nameof(ConcernType), ConcernType?.SelectedId, ConcernType?.SelectedSubId);

			return Page();
			
		}
	}
}