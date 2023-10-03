using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase
{
	public class CreateCaseBaseModel : AbstractPageModel
	{
		private readonly ITrustModelService _trustModelService;
		private readonly IUserStateCachedService _cachedUserService;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		[BindProperty(SupportsGet = true)]
		public TrustAddressModel TrustAddress { get; set; }

		public CreateCaseBaseModel(ITrustModelService trustModelService,
		IUserStateCachedService cachedUserService,
		IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_trustModelService = trustModelService;
			_cachedUserService = cachedUserService;
			_claimsPrincipalHelper = claimsPrincipalHelper;
		}

		protected async Task SetTrustAddress()
		{
			var userName = GetUserName();
			var userState = await _cachedUserService.GetData(userName);
			if (userState == null)
			{
				throw new Exception($"Could not retrieve cache for user '{userName}'");
			}

			if (string.IsNullOrEmpty(userState.TrustUkPrn))
			{
				throw new Exception($"Could not retrieve trust from cache for user '{userName}'");
			}

			var trustAddress = await _trustModelService.GetTrustAddressByUkPrn(userState.TrustUkPrn);

			TrustAddress = trustAddress ?? throw new Exception($"Could not find trust with UK PRN of {userState.TrustUkPrn}");
		}

		protected string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}
