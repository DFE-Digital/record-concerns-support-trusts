using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ConcernTypePageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<ConcernTypePageModel> _logger;
		private readonly ICachedService _cachedService;
		
		public CaseModel CaseModel { get; private set; }
		
		public ConcernTypePageModel(ITrustModelService trustModelService, ICachedService cachedService, 
			ITypeModelService typeModelService, ILogger<ConcernTypePageModel> logger)
		{
			_trustModelService = trustModelService;
			_typeModelService = typeModelService;
			_cachedService = cachedService;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("Case::ConcernTypePageModel::OnGetAsync");
				
				// Get cached data from case page.
				var userState = await GetUserState();
				
				// Fetch UI data
				CaseModel = new CaseModel {
					TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(userState.TrustUkPrn), 
					TypesDictionary = await _typeModelService.GetTypes() 
				};
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::ConcernTypePageModel::OnGetAsync::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogInformation("Case::ConcernTypePageModel::OnPost");

				var type = Request.Form["type"];
				var subType = Request.Form["subType"];
				var ragRating = Request.Form["riskRating"];
				var trustUkPrn = Request.Form["trust-ukprn"];
				var trustName = Request.Form["trust-name"];

				if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(ragRating)) 
					throw new Exception("Case::ConcernTypePageModel::Missing form values");
				
				var userState = await GetUserState();
				
				// Create a case post model
				var currentDate = DateTimeOffset.Now;
				userState.CreateCaseModel = new CreateCaseModel
				{
					Description = $"{type} {subType}",
					ClosedAt = currentDate,
					CreatedAt = currentDate,
					CreatedBy = User.Identity.Name,
					DeEscalation = currentDate,
					RagRating = ragRating,
					RecordType = type,
					ReviewAt = currentDate,
					UpdatedAt = currentDate,
					RecordSubType = subType,
					TrustUkPrn = trustUkPrn,
					TrustName = trustName,
					DirectionOfTravel = DirectionOfTravelEnum.Deteriorating.ToString()
				};
					
				// Store case model in cache for the details page
				await _cachedService.StoreData(User.Identity.Name, userState);
				
				return RedirectToPage("details");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::ConcernTypePageModel::OnPost::Exception - { ex.Message }");
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return Redirect("concerntype");
		}

		private async Task<UserState> GetUserState()
		{
			var userState = await _cachedService.GetData<UserState>(User.Identity.Name);
			if (userState is null)
			{
				throw new Exception("Case::ConcernTypePageModel::Cache CaseStateData is null");
			}

			return userState;
		}
	}
}