using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Records;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditRatingPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<EditRatingPageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public TypeModel TypeModel { get; private set; }

		[BindProperty(Name = "urn", SupportsGet = true)]
		public int CaseId { get; set; }

		[BindProperty(Name = "recordId", SupportsGet = true)]
		public int RecordId { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent ConcernRiskRating { get; set; }

		public EditRatingPageModel(ICaseModelService caseModelService, 
			IRecordModelService recordModelService,
			ITrustModelService trustModelService, 
			ITypeModelService typeModelService,
			ILogger<EditRatingPageModel> logger)
		{
			_caseModelService = caseModelService;
			_recordModelService = recordModelService;
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGet()
		{
			_logger.LogMethodEntered();
			
			try
			{
				await LoadPage(Request.Headers["Referer"].ToString());

			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
		
		public async Task<ActionResult> OnPostEditRiskRating(string url)
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					await LoadPage(url);
					return Page();
				}

				var ragRatingId = (ConcernRating)ConcernRiskRating.SelectedId.Value;

				// Create patch record model
				var patchRecordModel = new PatchRecordModel
				{
					UpdatedAt = DateTimeOffset.Now,
					Id = RecordId,
					CaseUrn = CaseId,
					RatingId = (long)ragRatingId,
					CreatedBy = User.Identity.Name
				};

				await _caseModelService.PatchRecordRating(patchRecordModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}
		
		private async Task LoadPage(string url)
		{
			CaseModel = await _caseModelService.GetCaseByUrn(CaseId);
			var recordModel = await _recordModelService.GetRecordModelById(CaseId, RecordId);
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
			TypeModel = await _typeModelService.GetSelectedTypeModelById(recordModel.TypeId);
			CaseModel.PreviousUrl = url;

			ConcernRiskRating = CaseComponentBuilder.BuildRiskToTrust(nameof(ConcernRiskRating), (int?)recordModel.RatingId);
		} 
	}
}