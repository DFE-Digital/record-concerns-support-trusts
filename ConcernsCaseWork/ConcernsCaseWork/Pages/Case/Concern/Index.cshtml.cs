using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Services.MeansOfReferral;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Services.Types;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;
using ConcernsCaseWork.CoreTypes;
using ConcernsCaseWork.Services.Cases;

namespace ConcernsCaseWork.Pages.Case.Concern
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly IRatingModelService _ratingModelService;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _cachedService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private TelemetryClient _telemetryClient;
		private ICaseModelService _caseModelService;
		
		public TrustAddressModel TrustAddress { get; private set; }

		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }

		[BindProperty]
		public RadioButtonsUiComponent ConcernType { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent MeansOfReferral { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent ConcernRiskRating { get; set; }

		public CaseModel CaseModel { get; private set; }

		public IndexPageModel(ITrustModelService trustModelService,
			IUserStateCachedService cachedService,
			IRatingModelService ratingModelService,
			IClaimsPrincipalHelper claimsPrincipalHelper,
			ILogger<IndexPageModel> logger,
			TelemetryClient telemetryClient, 
			ICaseModelService caseModelService)
		{
			_ratingModelService = Guard.Against.Null(ratingModelService);
			_trustModelService = Guard.Against.Null(trustModelService);
			_cachedService = Guard.Against.Null(cachedService);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			_logger = Guard.Against.Null(logger);
			_telemetryClient = Guard.Against.Null(telemetryClient);
			_caseModelService = Guard.Against.Null(caseModelService);
		}
		
		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();
				
			try
			{
				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}
			return Page();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}

				await GetCaseModel();
				
				var ragRatingId = (ConcernRating)ConcernRiskRating.SelectedId.Value;

				if (!Enum.IsDefined(typeof(ConcernRating), ragRatingId))
					throw new InvalidOperationException($"Unrecognised risk to trust {ragRatingId}");

				var ragRatingName = ragRatingId.Description();

				var typeId = (ConcernType)(ConcernType.SelectedSubId.HasValue ? ConcernType.SelectedSubId : ConcernType.SelectedId);

				if (!Enum.IsDefined(typeof(ConcernType), typeId))
					throw new InvalidOperationException($"Unrecognised concern type {typeId}");

				// Redis state
				var userState = await GetUserState();

				// Create a case model
				if (!userState.CreateCaseModel.CreateRecordsModel.Any())
				{
					var currentDate = DateTimeOffset.Now;
					userState.CreateCaseModel = new CreateCaseModel
					{
						CreatedAt = currentDate,
						ReviewAt = currentDate,
						UpdatedAt = currentDate,
						CreatedBy = GetUserName(),
						DeEscalation = currentDate,
						RagRatingName = ragRatingName,
						RagRating = RatingMapping.FetchRag(ragRatingName),
						RagRatingCss = RatingMapping.FetchRagCss(ragRatingName),
						DirectionOfTravel = DirectionOfTravelEnum.Deteriorating.ToString(),
						TrustUkPrn = userState.TrustUkPrn
					};
				}
				
				var createRecordModel = new CreateRecordModel
				{
					TypeId = (long)typeId,
					Type = typeId.Description(),
					SubType = null,
					RatingId = (long)ragRatingId,
					RatingName = ragRatingName,
					RagRating = RatingMapping.FetchRag(ragRatingName),
					RagRatingCss = RatingMapping.FetchRagCss(ragRatingName),
					MeansOfReferralId = MeansOfReferral.SelectedId.Value
				};
				string json = JsonSerializer.Serialize(createRecordModel);
				userState.CreateCaseModel.CreateRecordsModel.Add(createRecordModel);
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "ADD CONCERN",
					EventDescription = "Adding a concern",
					EventPayloadJson = json,
					EventUserName = userState.UserName
				});
				
				// Store case model in cache for the details page
				await _cachedService.StoreData(GetUserName(), userState);
				if (CaseModel!=null && !CaseModel.IsConcernsCase())
				{
					return RedirectToPage("/case/concern/add",new {urn = CaseModel.Urn});
				}
				return RedirectToPage("add");

				return RedirectToPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}
			
			return Page();
		}

		public async Task<ActionResult> OnGetCancel()
		{
			_logger.LogMethodEntered();
			
			try
			{
				var userState = await GetUserState();
				userState.CreateCaseModel = new CreateCaseModel();
				await _cachedService.StoreData(User.Identity?.Name, userState);
				
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		private async Task<ActionResult> LoadPage()
		{
			var userState = await GetUserState();
			var trustUkPrn = userState.TrustUkPrn;
		
			if (string.IsNullOrEmpty(trustUkPrn))
				throw new Exception("Cache TrustUkprn is null");
		
			TrustAddress = await _trustModelService.GetTrustAddressByUkPrn(trustUkPrn);
			CreateRecordsModel = new List<CreateRecordModel>();
			var ratingsModel = await _ratingModelService.GetRatingsModel();

			MeansOfReferral = CaseComponentBuilder.BuildMeansOfReferral(nameof(MeansOfReferral), MeansOfReferral?.SelectedId);
			ConcernRiskRating = CaseComponentBuilder.BuildConcernRiskRating(nameof(ConcernRiskRating), ratingsModel, ConcernRiskRating?.SelectedId);
			ConcernType = CaseComponentBuilder.BuildConcernType(nameof(ConcernType), ConcernType?.SelectedId, ConcernType?.SelectedSubId);

			await GetCaseModel();

			AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
			{
				EventName = "ADD CONCERN",
				EventDescription = "Loading add concern page",
				EventPayloadJson = "",
				EventUserName = userState.UserName
			});

			return Page();
		}

		private async Task GetCaseModel()
		{
			var caseUrnValue = RouteData.Values["urn"];
			long caseUrn = 0;
			if (caseUrnValue is null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0) ;
			if (caseUrn > 0)
			{
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
			}
		}

		private async Task<UserState> GetUserState()
		{
			var userState = await _cachedService.GetData(GetUserName());
			if (userState is null)
				throw new Exception("Cache CaseStateData is null");
			
			return userState;
		}
		
		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}