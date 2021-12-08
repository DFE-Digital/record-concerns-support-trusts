using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DetailsPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<DetailsPageModel> _logger;
		private readonly ICachedService _cachedService;
		
		public CreateCaseModel CreateCaseModel { get; private set; }
		
		public DetailsPageModel(ICaseModelService caseModelService, 
			ICachedService cachedService, ILogger<DetailsPageModel> logger)
		{
			_caseModelService = caseModelService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::DetailsPageModel::OnGetAsync");
				
				// Get cached data from case page.
				var caseStateModel = await GetUserState();

				// Fetch UI data
				CreateCaseModel = caseStateModel.CreateCaseModel;
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::DetailsPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogInformation("Case::DetailsPageModel::OnPostAsync");
				
				var issue = Request.Form["issue"];
				var currentStatus = Request.Form["current-status"];
				var nextSteps = Request.Form["next-steps"];
				var caseAim = Request.Form["case-aim"];
				var deEscalationPoint = Request.Form["de-escalation-point"];

				if (string.IsNullOrEmpty(issue)) 
					throw new Exception("Case::DetailsPageModel::Missing form values");
				
				// Complete create case model
				var userState = await GetUserState();
				
				var createCaseModel = userState.CreateCaseModel;
				createCaseModel.Issue = issue;
				createCaseModel.CurrentStatus = currentStatus;
				createCaseModel.NextSteps = nextSteps;
				createCaseModel.CaseAim = caseAim;
				createCaseModel.DeEscalationPoint = deEscalationPoint;
					
				var caseUrn = await _caseModelService.PostCase(createCaseModel);
				
				return RedirectToPage("Management", new { urn = caseUrn });
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::DetailsPageModel::OnPostAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return Redirect("details");
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _cachedService.GetData<UserState>(User.Identity.Name);
			if (userState is null)
				throw new Exception("Case::DetailsPageModel::Cache CaseStateData is null");
			
			return userState;
		}
	}
}