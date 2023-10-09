using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase
{
	public class SelectRegionModel : AbstractPageModel
    {
		private readonly ILogger<SelectRegionModel> _logger;
		private readonly IUserStateCachedService _userStateCache;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ITrustModelService _trustModelService;

		public TrustDetailsModel TrustDetailsModel { get; set; }
		public IList<CreateRecordModel> CreateRecordsModel { get; set; }
		public CreateCaseModel CreateCaseModel { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent Region { get; set; }

		public SelectRegionModel(
			ITrustModelService trustModelService,
			IUserStateCachedService userStateCache,
			ILogger<SelectRegionModel> logger,
			IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_trustModelService = trustModelService;
			_logger = logger;
			_userStateCache = userStateCache;
			_claimsPrincipalHelper = claimsPrincipalHelper;
		}

        public async Task<IActionResult> OnGet()
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
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					await LoadPage();
					return Page();
				}

				var userState = await GetUserState();
				userState.CreateCaseModel.Territory = (Territory)Region.SelectedId;
				await _userStateCache.StoreData(GetUserName(), userState);

				return Redirect("/case/concern");
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
			var userState = await GetUserState();
			var trustUkPrn = userState.TrustUkPrn;

			CreateCaseModel = userState.CreateCaseModel;
			CreateRecordsModel = userState.CreateCaseModel.CreateRecordsModel;
			TrustDetailsModel = await _trustModelService.GetTrustByUkPrn(trustUkPrn);
			Region = CaseComponentBuilder.BuildRegion(nameof(Region), Region?.SelectedId);
		}

		private async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(GetUserName());
			if (userState is null)
				throw new Exception("Could not retrieve cached new case data for user");

			return userState;
		}

		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}
