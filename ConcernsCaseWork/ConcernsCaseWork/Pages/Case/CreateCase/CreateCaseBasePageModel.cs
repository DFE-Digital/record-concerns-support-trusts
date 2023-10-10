using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.CreateCase
{
	public class CreateCaseBasePageModel : AbstractPageModel
	{
		private readonly IUserStateCachedService _userStateCache;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		public CreateCaseBasePageModel(
			IUserStateCachedService userStateCache,
			IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_userStateCache = userStateCache;
			_claimsPrincipalHelper = claimsPrincipalHelper;
		}

		protected async Task<UserState> GetUserState()
		{
			var userState = await _userStateCache.GetData(GetUserName());
			if (userState is null)
				throw new Exception("Could not retrieve cached new case data for user");

			return userState;
		}

		protected string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}
