using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using System.Threading.Tasks;
using System;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Pages.Case.CreateCase
{
	public class CreateCaseBasePageModel : AbstractPageModel
	{
		private readonly ILogger<SelectRegionModel> _logger;
		private readonly IUserStateCachedService _userStateCache;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private readonly ITrustModelService _trustModelService;

		public CreateCaseBasePageModel(
			ITrustModelService trustModelService,
			IUserStateCachedService userStateCache,
			ILogger<SelectRegionModel> logger,
			IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			trustModelService = _trustModelService;
			logger = _logger;
			userStateCache = _userStateCache;
			claimsPrincipalHelper = _claimsPrincipalHelper;
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
