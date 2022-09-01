using Ardalis.GuardClauses;
using Service.Redis.Base;
using Service.Redis.Models;
using System;
using System.Threading.Tasks;

namespace Service.Redis.Users
{
	public class UserStateCachedService : CachedService, IUserStateCachedService
	{
		public UserStateCachedService(ICacheProvider cacheProvider) : base(cacheProvider)
		{
		}

		public Task<UserState> GetData(string userIdentity)
		{
			Guard.Against.NullOrWhiteSpace(userIdentity);

			return GetData<UserState>(userIdentity);
		}

		public async Task StoreData(string userIdentity, UserState userState)
		{
			Guard.Against.NullOrWhiteSpace(userIdentity);

			// no need to store nulls
			await (userState is null
				? ClearData(userIdentity)
				: base.StoreData(userIdentity, userState));
		}
	}
}
