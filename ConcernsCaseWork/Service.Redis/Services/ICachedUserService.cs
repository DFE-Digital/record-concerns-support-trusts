using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Services
{
	public interface ICachedUserService
	{
		Task<UserClaims> GetUserAsync(UserCredentials userCredentials);
	}
}