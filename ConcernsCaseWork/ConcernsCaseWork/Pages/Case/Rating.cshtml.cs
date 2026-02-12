using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Case.CreateCase;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using ConcernsCaseWork.Utils.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class RatingPageModel : CreateCaseBasePageModel
	{
		private readonly ILogger<RatingPageModel> _logger;
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private TelemetryClient _telemetryClient;
		
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public CreateCaseModel CreateCaseModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }

		[BindProperty]
		public RadioButtonsUiComponent RiskToTrust { get; set; }

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int? CaseUrn { get; set; }

		[BindProperty]
		public bool? YesCheckedRagRational { get; set; }

		[BindProperty]
		public string RatingRationalCommentary { get; set; }

		public static int RationYesCommentaryMaxLength => 250;

		public RatingPageModel(ITrustModelService trustModelService, 
			IUserStateCachedService userStateCache,
			ILogger<RatingPageModel> logger, 
			IClaimsPrincipalHelper claimsPrincipalHelper,
			TelemetryClient telemetryClient) : base(userStateCache, claimsPrincipalHelper)
		{
			_trustModelService = trustModelService;
			_userStateCache = userStateCache;
			_logger = logger;
			_telemetryClient = telemetryClient;
		}
		
		public async Task OnGetAsync()
		{
			_logger.LogMethodEntered();
				
			// Fetch UI data
			await LoadPage();
		}
		
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (YesCheckedRagRational == null)
				{
					ModelState.AddModelError(nameof(YesCheckedRagRational), "Select RAG rationale");
				}
				else if (YesCheckedRagRational is true)
				{
					if (string.IsNullOrWhiteSpace(RatingRationalCommentary))
					{
						ModelState.AddModelError(nameof(RatingRationalCommentary), "You must enter a RAG rationale commentary");
					}
					else if (RatingRationalCommentary.Length > RationYesCommentaryMaxLength)
					{
						ModelState.AddModelError("RationalCommentaryMaxLength", $"You have {RatingRationalCommentary.Length - RationYesCommentaryMaxLength} characters too many.");
					}
				}

				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}

				var ragRatingId = (ConcernRating)RiskToTrust.SelectedId.Value;

				if (!Enum.IsDefined(typeof(ConcernRating), ragRatingId))
					throw new InvalidOperationException($"Unrecognised risk to trust {ragRatingId}");

				var ragRatingName = ragRatingId.Description();

				// Redis state
				var userState = await GetUserState();

				// Update cache model
				userState.CreateCaseModel.RatingId = (long)ragRatingId;

				userState.CreateCaseModel.RatingRational = YesCheckedRagRational.Value;
				userState.CreateCaseModel.RatingRationalCommentary = RatingRationalCommentary;

				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "CREATE CASE",
					EventDescription = $"Rating added {ragRatingName}",
					EventPayloadJson = "",
					EventUserName = userState.UserName
				});

				// Store case model in cache for the details page
				await _userStateCache.StoreData(GetUserName(), userState);

				if (CaseUrn.HasValue)
				{
					return RedirectToPage("details",new {urn = CaseUrn });
				}

				return RedirectToPage("details");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}
		
		private async Task<ActionResult> LoadPage()
		{
			try
			{
				var userState = await GetUserState();
				var trustUkPrn = userState.TrustUkPrn;

				if (string.IsNullOrEmpty(trustUkPrn)) 
					throw new Exception("Cache TrustUkprn is null");

				CreateCaseModel = userState.CreateCaseModel;
				CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
				TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "CREATE CASE",
					EventDescription = "Rating being added",
					EventPayloadJson = "",
					EventUserName = userState.UserName
				});

				RiskToTrust = CaseComponentBuilder.BuildRiskToTrust(nameof(RiskToTrust), RiskToTrust?.SelectedId);

				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
	}
}