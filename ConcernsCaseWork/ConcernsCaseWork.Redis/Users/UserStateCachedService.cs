using Ardalis.GuardClauses;
using ConcernsCaseWork.Redis.Base;
using ConcernsCaseWork.Redis.Models;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Users
{
	public class UserStateCachedService : CachedService, IUserStateCachedService
	{
		private const int _defaultCacheExpiryHours = 9;

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
				: base.StoreData(userIdentity, userState, _defaultCacheExpiryHours));
		}
	}
}
