using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Trusts;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase.NonConcernsCase
{
	public class DetailsPageModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<DetailsPageModel> _logger;
		private readonly ITrustService _trustService;
		private readonly IUserStateCachedService _userStateCache;

		public CreateCaseModel CreateCaseModel { get; private set; }
		public TrustDetailsModel TrustDetailsModel { get; private set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; private set; }

		[BindProperty]
		public TextAreaUiComponent CaseHistory { get; set; }

		public DetailsPageModel(ICaseModelService caseModelService,
			ITrustModelService trustModelService,
			IUserStateCachedService userStateCache,
			ILogger<DetailsPageModel> logger,
			ITrustService trustService)
		{
			_trustModelService = Guard.Against.Null(trustModelService);
			_caseModelService = Guard.Against.Null(caseModelService);
			_userStateCache = Guard.Against.Null(userStateCache);
			_logger = Guard.Against.Null(logger);
			_trustService = Guard.Against.Null(trustService);
		}

		public async Task<ActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				await LoadPageData();
				LoadComponents();

				return Page();
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
					await LoadPageData();
					LoadComponents();

					return Page();
				}

				var createCaseModel = await BuildCreateCaseModel();

				var newCaseId = await _caseModelService.PostCase(createCaseModel);

				return Redirect($"/case/{newCaseId}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private async Task LoadPageData()
		{
			var userState = await GetUserState();
			var trustUkPrn = userState.TrustUkPrn;

			if (string.IsNullOrEmpty(trustUkPrn))
				throw new Exception("Cache TrustUkprn is null");

			CreateCaseModel = userState.CreateCaseModel;
			CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
		}

		private void LoadComponents()
		{
			CaseHistory = BuildCaseHistoryComponent(CaseHistory?.Text.StringContents);
		}

		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(User.Identity?.Name);
			if (userState is null)
				throw new Exception("Cache CaseStateData is null");

			return userState;
		}

		private TextAreaUiComponent BuildCaseHistoryComponent(string contents = "")
		=> new("case-history", nameof(CaseHistory), "Case history (optional)")
		{
			HintText = "You can record any information that gives you additional context to the case",
			Text = new ValidateableString()
			{
				MaxLength = 4300,
				StringContents = contents,
				DisplayName = "Case history"
			}
		};

		private async Task<CreateCaseModel> BuildCreateCaseModel()
		{
			var userState = await GetUserState();

			var result = userState.CreateCaseModel;

			var caseHistory = CaseHistory.Text.StringContents;

			// get the trust being used for the case
			var trust = await _trustService.GetTrustByUkPrn(userState.TrustUkPrn);

			result.TrustUkPrn = trust.GiasData.UkPrn;
			result.CaseHistory = caseHistory;
			result.TrustCompaniesHouseNumber = trust.GiasData.CompaniesHouseNumber;

			result.StatusId = (long)CaseStatus.Live;
			result.RatingId = (long)ConcernRating.NotApplicable;
			result.CreatedBy = userState.UserName;

			var currentDate = DateTimeOffset.Now;
			result.CreatedAt = currentDate;
			result.UpdatedAt = currentDate;
			result.ReviewAt = currentDate;

			return result;
		}
	}
}
