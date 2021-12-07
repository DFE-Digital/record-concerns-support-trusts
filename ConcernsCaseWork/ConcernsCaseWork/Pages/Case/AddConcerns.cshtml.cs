using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Pages.Validators;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
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
	public class AddConcernsPageModel : AbstractPageModel
	{
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<AddConcernsPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ICachedService _cachedService;
		
		public TypeModel TypeModel { get; private set; }
		public IList<RatingModel> RatingsModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public IList<CreateRecordModel> CreateRecordsModel { get; set; }

		public AddConcernsPageModel(ITrustModelService trustModelService, 
			ICachedService cachedService, 
			ITypeModelService typeModelService, 
			IRatingModelService ratingModelService,
			ILogger<AddConcernsPageModel> logger)
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

				CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;

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

				// Form
				var type = Request.Form["type"].ToString();
				var subType = Request.Form["sub-type"].ToString();
				var ragRating = Request.Form["rating"].ToString();
				var trustName = Request.Form["trust-name"].ToString();
				trustUkPrn = Request.Form["trust-ukprn"].ToString();

				// Type
				(typeUrn, type, subType) = type.SplitType(subType);

				// Rating
				var splitRagRating = ragRating.Split(":");
				var ragRatingUrn = splitRagRating[0];
				var ragRatingName = splitRagRating[1];

				// Redis state
				var userState = await GetUserState();

				// Create a case model
				var currentDate = DateTimeOffset.Now;
				userState.CreateCaseModel = new CreateCaseModel
				{
					CreatedAt = currentDate,
					ReviewAt = currentDate,
					UpdatedAt = currentDate,
					ClosedAt = currentDate,
					CreatedBy = User.Identity.Name,
					DeEscalation = currentDate,
					RagRating = RatingMapping.FetchRag(ragRatingName),
					RagRatingCss = RatingMapping.FetchRagCss(ragRatingName),
					TrustUkPrn = trustUkPrn,
					TrustName = trustName,
					DirectionOfTravel = DirectionOfTravelEnum.Deteriorating.ToString()
				};

				//var createRecordModel = new CreateRecordModel
				//{
				//	CreatedAt = currentDate,
				//	UpdatedAt = currentDate,
				//	ReviewAt = currentDate,
				//	ClosedAt = currentDate,
				//	Type = type,
				//	SubType = subType,
				//	Reason = "Reason",
				//	TypeUrn = long.Parse(typeUrn),
				//	RatingUrn = long.Parse(ragRatingUrn),
				//	RagRatingName = ragRatingName,
				//	RagRating = RatingMapping.FetchRag(ragRatingName),
				//	RagRatingCss = RatingMapping.FetchRagCss(ragRatingName)
				//};

				//userState.CreateCaseModel.CreateRecordsModel.Add(createRecordModel);

				// Store case model in cache for the details page
				await _cachedService.StoreData(User.Identity.Name, userState);

				return RedirectToPage("addconcerns");
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

			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
			RatingsModel = await _ratingModelService.GetRatingsModel();
			TypeModel = await _typeModelService.GetTypeModel();

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