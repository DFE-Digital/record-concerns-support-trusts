using Ardalis.GuardClauses;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosurePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITrustModelService _trustModelService;
		private readonly ILogger<ClosurePageModel> _logger;
		private TelemetryClient _telemetryClient;
		private readonly IUserStateCachedService _userStateCache;

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseId { get; set; }

		[BindProperty]
		public TextAreaUiComponent RationaleForClosure { get; set; }

		public TrustDetailsModel TrustDetailsModel { get; private set; }

		public ClosurePageModel(
			ICaseModelService caseModelService, 
			ITrustModelService trustModelService,
			ILogger<ClosurePageModel> logger,
			IUserStateCachedService cachedService,
			TelemetryClient telemetryClient)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_trustModelService = Guard.Against.Null(trustModelService);
			_logger = Guard.Against.Null(logger);
			_telemetryClient = Guard.Against.Null(telemetryClient);
			_userStateCache = Guard.Against.Null(cachedService);
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				await LoadPage();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}
		}

		public async Task<IActionResult> OnPostCloseCase()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}

				var userState = await GetUserState();

				var patchCaseModel = new PatchCaseModel
				{
					// Update patch case model
					Urn = CaseId,
					ClosedAt = DateTimeOffset.Now,
					UpdatedAt = DateTimeOffset.Now,
					ReasonAtReview = RationaleForClosure.Text.StringContents
				};

				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "CASE CLOSED",
					EventDescription = $"Case has been closed {CaseId}",
					EventPayloadJson = JsonSerializer.Serialize(patchCaseModel),
					EventUserName = userState.UserName
				});

				await _caseModelService.PatchClosure(patchCaseModel);
				
				return Redirect("/");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private async Task LoadPage()
		{
			// Fetch UI data
			var caseModel = await _caseModelService.GetCaseByUrn(CaseId);

			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(caseModel.TrustUkPrn);
			RationaleForClosure = BuildRationaleForClosureComponent(RationaleForClosure?.Text.StringContents);
		}

		private TextAreaUiComponent BuildRationaleForClosureComponent(string contents = "")
		=> new("case-outcomes", nameof(RationaleForClosure), "Rationale for closure")
		{
			Text = new ValidateableString()
			{
				MaxLength = 200,
				StringContents = contents,
				DisplayName = "Rationale for closure",
				Required = true
			}
		};

		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(User.Identity.Name);
			if (userState is null)
				throw new Exception("Cache CaseStateData is null");
			
			return userState;
		}
	}
}