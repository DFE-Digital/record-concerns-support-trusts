using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ConcernsCaseWork.Service.Trusts;
using System;
using System.Net;
using System.Threading.Tasks;
using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using Microsoft.ApplicationInsights;
using System.Text.Json;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _userStateCache;
		private readonly ILogger<IndexPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private TelemetryClient _telemetry;
		
		private const int _searchQueryMinLength = 3;
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage);



		[BindProperty]
		public FindTrustModel FindTrustModel { get; set; }

		public IndexPageModel(ITrustModelService trustModelService,
			IUserStateCachedService userStateCache,
			ILogger<IndexPageModel> logger,
			IClaimsPrincipalHelper claimsPrincipalHelper,
			TelemetryClient telemetryClient)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			FindTrustModel = new();
			_telemetry = telemetryClient;
		}

		public async Task<ActionResult> OnPost()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					return Page();
				}

				// Double check selected trust.
				if (string.IsNullOrEmpty(FindTrustModel?.SelectedTrustUkprn) ||
				    FindTrustModel.SelectedTrustUkprn.Contains("-") ||
				    FindTrustModel.SelectedTrustUkprn.Length < _searchQueryMinLength)
				{
					throw new Exception($"Selected trust is incorrect - {FindTrustModel?.SelectedTrustUkprn}");
				}

				// Store CaseState into cache.
				var userState = await _userStateCache.GetData(GetUserName()) ?? new UserState(GetUserName());
				userState.TrustUkPrn = FindTrustModel.SelectedTrustUkprn;
				userState.CreateCaseModel = new CreateCaseModel();
				AppInsightsHelper.LogEvent(_telemetry, new AppInsightsModel()
				{
					EventName = "CREATE CASE",
					EventDescription = "Case index page access",
					EventPayloadJson = "",
					EventUserName = userState.UserName
				});
				await _userStateCache.StoreData(GetUserName(), userState);
				return Redirect(Url.Page("Concern/Index"));
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}