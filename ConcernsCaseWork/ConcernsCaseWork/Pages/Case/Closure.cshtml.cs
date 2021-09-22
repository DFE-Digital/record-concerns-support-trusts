using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.TRAMS.Status;
using System;
using ConcernsCaseWork.Extensions;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosurePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<ClosurePageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		
		public ClosurePageModel(ICaseModelService caseModelService, ILogger<ClosurePageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::ClosurePageModel::OnGetAsync");
				
				// Fetch case urn
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("Case::ClosurePageModel::CaseUrn is null or invalid to parse");
				}

				// Fetch UI data
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::ClosurePageModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public async Task<IActionResult> OnPostCloseCase()
		{
			try
			{
				_logger.LogInformation("Case::ClosurePageModel::OnPostCloseCase");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("ClosurePageModel::CaseUrn is null or invalid to parse");
				}
				
				var caseOutcomes = Request.Form["case-outcomes"];
				var monitoring = Request.Form["monitoring"];
				var dayToReview = Request.Form["dtr-day"];
				var monthToReview = Request.Form["dtr-month"];
				var yearToReview = Request.Form["dtr-year"];

				if (string.IsNullOrEmpty(caseOutcomes)) throw new Exception("Missing form values");
				
				var isMonitoring = monitoring.ToString().ToBoolean();
				var patchCaseModel = new PatchCaseModel();
					
				if (isMonitoring
				    && !string.IsNullOrEmpty(dayToReview)
				    && !string.IsNullOrEmpty(monthToReview)
				    && !string.IsNullOrEmpty(yearToReview))
				{
					// Validate date
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
				patchCaseModel.StatusName = isMonitoring ? Status.Monitoring.ToString() : Status.Close.ToString();
				patchCaseModel.ReasonAtReview = caseOutcomes;
				
				await _caseModelService.PatchClosure(patchCaseModel);
					
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::ClosurePageModel::OnPostCloseCase::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Redirect("closure");
		}
	}
}