using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Services
{
	public interface ICacheUserService
	{
		Task<UserClaims> GetCachedUser(string key);
		Task ClearCache(string key);
	}
}