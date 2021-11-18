using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Rating;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ConcernTypePageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ICachedService _cachedService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<ConcernTypePageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		
		public ConcernTypePageModel(ITrustModelService trustModelService, ICachedService cachedService, 
			ITypeModelService typeModelService, IRatingModelService ratingModelService,
			ILogger<ConcernTypePageModel> logger)
		{
			_ratingModelService = ratingModelService;
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
				await LoadPage(userState.TrustUkPrn);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::ConcernTypePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			var trustUkPrn = string.Empty;
			
			try
			{
				_logger.LogInformation("Case::ConcernTypePageModel::OnPostAsync");

				var type = Request.Form["type"];
				var subType = Request.Form["subType"];
				var ragRating = Request.Form["ragRating"];
				var trustName = Request.Form["trustName"];
				trustUkPrn = Request.Form["trustUkprn"];
				
				if (!IsValidConcernType(type, ref subType, ragRating, trustUkPrn))
					throw new Exception("Case::ConcernTypePageModel::Missing form values");

				var splitRagRating = ragRating.ToString().Split(":");
				var ragRatingUrn = splitRagRating[0];
				var ragRatingName = splitRagRating[1];
				
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
					RagRatingName = ragRatingName,
					RagRatingUrn = long.Parse(ragRatingUrn),
					RagRating = RatingMapping.FetchRag(ragRatingName),
					RagRatingCss = RatingMapping.FetchRagCss(ragRatingName),
					Type = type,
					ReviewAt = currentDate,
					UpdatedAt = currentDate,
					SubType = subType,
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
				_logger.LogError("Case::ConcernTypePageModel::OnPostAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return await LoadPage(trustUkPrn);
		}

		private async Task<ActionResult> LoadPage(string trustUkPrn)
		{
			if (string.IsNullOrEmpty(trustUkPrn))
			{
				CaseModel = new CaseModel { 
					TypesDictionary = await _typeModelService.GetTypes()
				};
				RatingsModel = await _ratingModelService.GetRatings();
			}
			else
			{
				CaseModel = new CaseModel { TypesDictionary = await _typeModelService.GetTypes() };
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
				RatingsModel = await _ratingModelService.GetRatings();
			}
			
			return Page();
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