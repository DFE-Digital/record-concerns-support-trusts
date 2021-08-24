using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Users
{
	public interface IUserCachedService
	{
		Task<UserClaims> GetUserAsync(UserCredentials userCredentials);
	}
}