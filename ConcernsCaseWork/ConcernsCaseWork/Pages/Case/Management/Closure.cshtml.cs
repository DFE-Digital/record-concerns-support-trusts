using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConcernsCasework.Service.Status;
using System;
using ConcernsCaseWork.Services.Trusts;
using System.Threading.Tasks;
using ConcernsCaseWork.Services.Records;
using Service.Redis.Status;
using System.Linq;
using System.Collections.Generic;
using ConcernsCaseWork.Services.FinancialPlan;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Services.NtiWarningLetter;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosurePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly IRecordModelService _recordModelService;
		private readonly IStatusCachedService _statusCachedService;
		private readonly ISRMAService _srmaModelService;
		private readonly IFinancialPlanModelService _financialPlanModelService;
		private readonly INtiUnderConsiderationModelService _ntiUnderConsiderationModelService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly ILogger<ClosurePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		
		public ClosurePageModel(ICaseModelService caseModelService, ITrustModelService trustModelService, IRecordModelService recordModelService, 
			IStatusCachedService statusCachedService, ISRMAService srmaModelService, IFinancialPlanModelService financialPlanModelService, INtiUnderConsiderationModelService ntiUnderConsiderationModelService, INtiWarningLetterModelService ntiWarningLetterModelService, ILogger<ClosurePageModel> logger)
		{
			_caseModelService = caseModelService;
			_trustModelService = trustModelService;
			_recordModelService = recordModelService;
			_statusCachedService = statusCachedService;
			_srmaModelService = srmaModelService;
			_financialPlanModelService = financialPlanModelService;
			_ntiUnderConsiderationModelService = ntiUnderConsiderationModelService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::ClosurePageModel::OnGetAsync");
				
				// Fetch case urn
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn) || caseUrn == 0)
				{
					throw new Exception("CaseUrn is null or invalid to parse");
				}

				var validationMessages = await ValidateCloseConcern(caseUrn);

				if (validationMessages.Count > 0)
				{
					TempData["OpenActions.Message"] = validationMessages;
					return;
				}

				// Fetch UI data
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(CaseModel.TrustUkPrn);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ClosurePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public async Task<IActionResult> OnPostCloseCase()
		{
			try
			{
				_logger.LogInformation("Case::ClosurePageModel::OnPostCloseCase");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn) || caseUrn == 0)
				{
					throw new Exception("CaseUrn is null or invalid to parse");
				}

				if (!(await IsCaseAlreadyClosed(User.Identity.Name, caseUrn)))
				{
					var caseOutcomes = Request.Form["case-outcomes"];
					if (string.IsNullOrEmpty(caseOutcomes))
					{
						throw new Exception("Missing form values");
					}

					var patchCaseModel = new PatchCaseModel
					{
						// Update patch case model
						Urn = caseUrn,
						CreatedBy = User.Identity.Name,
						ClosedAt = DateTimeOffset.Now,
						UpdatedAt = DateTimeOffset.Now,
						StatusName = StatusEnum.Close.ToString(),
						ReasonAtReview = caseOutcomes
					};

					await _caseModelService.PatchClosure(patchCaseModel);
				}
					
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ClosurePageModel::OnPostCloseCase::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Redirect("closure");
		}

		private async Task<bool> IsCaseAlreadyClosed(string userName, long urn)
		{
			var closedState = await _statusCachedService.GetStatusByName(StatusEnum.Close.ToString());
			var caseDto = await _caseModelService.GetCaseByUrn(userName, urn);

			return closedState != null && caseDto?.StatusUrn == closedState?.Urn;
		}


		private async Task<List<string>> ValidateCloseConcern(long caseUrn)
		{
			List<string> errorMessages = new List<string>();

			var recordsModels = await _recordModelService.GetRecordsModelByCaseUrn(User.Identity.Name, caseUrn);
			var liveStatus = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
			var numberOfOpenConcerns = recordsModels.Count(r => r.StatusUrn.CompareTo(liveStatus.Urn) == 0);
			
			var srmaModels = (await _srmaModelService.GetSRMAsForCase(caseUrn)).ToList();
			var financialPlanModels = (await _financialPlanModelService.GetFinancialPlansModelByCaseUrn(caseUrn, User.Identity.Name)).ToList();
			var ntiUnderConsiderationModels = (await _ntiUnderConsiderationModelService.GetNtiUnderConsiderationsForCase(caseUrn)).ToList();
			var ntiWarningLetterModels = (await _ntiWarningLetterModelService.GetNtiWarningLettersForCase(caseUrn)).ToList();

			var hasOpenSRMAs = srmaModels.Any(srma => srma.ClosedAt == null) == true;
			var hasOpenFinancialPlans = financialPlanModels.Any(fp => fp.ClosedAt == null) == true;
			var hasOpenNTIUnderConsiderations = ntiUnderConsiderationModels.ToList().Any(uc => uc.ClosedAt == null) == true;
			var hasOpenNTIWarningLetters = ntiWarningLetterModels.ToList().Any(uc => uc.ClosedAt == null) == true;


			if (numberOfOpenConcerns > 0)
			{
				errorMessages.Add("Resolve Concerns");
			}

			if (hasOpenSRMAs)
			{
				errorMessages.Add("Resolve SRMA");
			}

			if(hasOpenFinancialPlans)
			{
				errorMessages.Add("Resolve Financial Plan");
			}

			if (hasOpenNTIUnderConsiderations)
			{
				errorMessages.Add("Close NTI Under Consideration");
			}

			if (hasOpenNTIWarningLetters)
			{
				errorMessages.Add("Resolve NTI Warning Letter");
			}

			if (await IsCaseAlreadyClosed(User.Identity.Name, caseUrn))
			{
				errorMessages.Add("This case is already closed.");
			}


			return errorMessages;
		}
	}
}