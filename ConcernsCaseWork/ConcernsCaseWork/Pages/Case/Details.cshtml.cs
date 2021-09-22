using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class DetailsPageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<DetailsPageModel> _logger;
		private readonly ICachedService _cachedService;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IDictionary<string, IList<string>> TypesDictionary { get; private set; }

		public DetailsPageModel(ITrustModelService trustModelService, ICaseModelService caseModelService, 
			ICachedService cachedService, ITypeModelService typeModelService, ILogger<DetailsPageModel> logger)
		{
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
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
				var caseStateModel = await _cachedService.GetData<UserState>(User.Identity.Name);
				if (caseStateModel is null)
				{
					throw new Exception("Cache CaseStateData is null");
				}

				// Fetch UI data
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseStateModel.TrustUkPrn);
				TypesDictionary = await _typeModelService.GetTypes();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::DetailsPageModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task<IActionResult> OnPost()
		{
			try
			{
				_logger.LogInformation("Case::DetailsPageModel::OnPost");

				var type = Request.Form["type"];
				var subType = Request.Form["subType"];
				var ragRating = Request.Form["riskRating"];
				var issueDetail = Request.Form["issue-detail"];
				var currentStatusDetail = Request.Form["current-status-detail"];
				var nextStepsDetail = Request.Form["next-steps-detail"];
				var resolutionStrategyDetail = Request.Form["resolution-strategy-detail"];
				var trustUkPrn = Request.Form["trust-Ukprn"];

				if (string.IsNullOrEmpty(type) 
				    || string.IsNullOrEmpty(ragRating) 
				    || string.IsNullOrEmpty(issueDetail)) throw new Exception("Missing form values");
				
				// Create a case post model
				var currentDate = DateTimeOffset.Now;
				var createCaseModel = new CreateCaseModel
				{
					Description = $"{type} {subType}",
					Issue = issueDetail,
					ClosedAt = currentDate,
					CreatedAt = currentDate,
					CreatedBy = User.Identity.Name,
					CurrentStatus = currentStatusDetail,
					DeEscalation = currentDate,
					NextSteps = nextStepsDetail,
					RagRating = ragRating,
					RecordType = type,
					ResolutionStrategy = resolutionStrategyDetail,
					ReviewAt = currentDate,
					UpdatedAt = currentDate,
					RecordSubType = subType,
					TrustUkPrn = trustUkPrn
				};
					
				var newCase = await _caseModelService.PostCase(createCaseModel);
					
				return RedirectToPage("Management", new { id = newCase.Urn });
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::DetailsPageModel::OnPost::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return Redirect("details");
		}
	}
}