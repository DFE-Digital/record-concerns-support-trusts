using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Status;
using System;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Services.Trusts;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosurePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<ClosurePageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		
		public ClosurePageModel(ICaseModelService caseModelService, ITrustModelService trustModelService, ILogger<ClosurePageModel> logger)
		{
			_caseModelService = caseModelService;
			_trustModelService = trustModelService;
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
					throw new Exception("Case::ClosurePageModel::CaseUrn is null or invalid to parse");
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
					throw new Exception("ClosurePageModel::CaseUrn is null or invalid to parse");
				}
				
				if(!ClosureValidator.IsValid(Request.Form))
					throw new Exception("Case::ClosurePageModel::Missing form values");
				
				var caseOutcomes = Request.Form["case-outcomes"];
				var monitoring = Request.Form["monitoring"];
				var dayToReview = Request.Form["dtr-day"];
				var monthToReview = Request.Form["dtr-month"];
				var yearToReview = Request.Form["dtr-year"];
				
				var isMonitoring = monitoring.ToString().ToBoolean();
				var patchCaseModel = new PatchCaseModel();
					
				if (isMonitoring)
				{
					// Convert parts
					var year = int.Parse(yearToReview);
					var month = int.Parse(monthToReview);
					var day = int.Parse(dayToReview);
						
					DateTime sourceDate = new DateTime(year, month, day, 0, 0, 0);
					DateTime utcTime = DateTime.SpecifyKind(sourceDate, DateTimeKind.Utc);
						
					// Set review at
					patchCaseModel.ReviewAt = new DateTimeOffset(utcTime);
				}
				else
				{
					patchCaseModel.ClosedAt = DateTimeOffset.Now;
				}
					
				// Update patch case model
				patchCaseModel.Urn = caseUrn;
				patchCaseModel.CreatedBy = User.Identity.Name;
				patchCaseModel.UpdatedAt = DateTimeOffset.Now;
				patchCaseModel.StatusName = isMonitoring ? StatusEnum.Monitoring.ToString() : StatusEnum.Close.ToString();
				patchCaseModel.ReasonAtReview = caseOutcomes;
				
				await _caseModelService.PatchClosure(patchCaseModel);
					
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ClosurePageModel::OnPostCloseCase::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Redirect("closure");
		}
	}
}