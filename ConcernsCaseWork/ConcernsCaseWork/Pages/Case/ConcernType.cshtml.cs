using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Validators;
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
		private readonly ILogger<ConcernTypePageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ICachedService _cachedService;
		
		public CaseModel CaseModel { get; private set; }
		public TypeModel TypeModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		
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

				if (!ConcernTypeValidator.IsValid(Request.Form))
					throw new Exception("Case::ConcernTypePageModel::Missing form values");

				string typeUrn;
				var type = Request.Form["type"].ToString();
				var subType = Request.Form["sub-type"].ToString();
				var ragRating = Request.Form["ragRating"].ToString();
				var trustName = Request.Form["trust-name"].ToString();
				trustUkPrn = Request.Form["trust-ukprn"].ToString();
				(typeUrn, type, subType) = type.SplitType(subType);
				
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
					RagRatingName = ragRating,
					RagRating = RagMapping.FetchRag(ragRating),
					RagRatingCss = RagMapping.FetchRagCss(ragRating),
					Type = type,
					TypeUrn = long.Parse(typeUrn),
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
			if (string.IsNullOrEmpty(trustUkPrn)) return Page();
			
			// TODO remove when rating code is merged in
			CaseModel = new CaseModel();
			TypeModel = await _typeModelService.GetTypeModel();
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);

			return Page();
		}
		
		private async Task<UserState> GetUserState()
		{
			var userState = await _cachedService.GetData<UserState>(User.Identity.Name);
			if (userState is null)
				throw new Exception("Case::ConcernTypePageModel::Cache CaseStateData is null");
			
			return userState;
		}
	}
}