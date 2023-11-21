using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
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

		public CaseModel CaseModel { get; private set; }

		public IndexPageModel(ICaseModelService caseModelService,
			IRecordModelService recordModelService,
			ITrustModelService trustModelService,
			ILogger<IndexPageModel> logger)
		{
			_recordModelService = recordModelService;
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

				var createRecordModel = new CreateRecordModel
				{
					CaseUrn = CaseUrn,
					TypeId = (long)ConcernType.SelectedId,
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

			MeansOfReferral = CaseComponentBuilder.BuildMeansOfReferral(caseModel.Division, nameof(MeansOfReferral), MeansOfReferral?.SelectedId);
			ConcernRiskRating = CaseComponentBuilder.BuildConcernRiskRating(nameof(ConcernRiskRating), ConcernRiskRating?.SelectedId);
			ConcernType = CaseComponentBuilder.BuildConcernType(caseModel.Division, nameof(ConcernType), ConcernType?.SelectedId);

			CaseModel = caseModel;

			return Page();
			
		}
	}
}