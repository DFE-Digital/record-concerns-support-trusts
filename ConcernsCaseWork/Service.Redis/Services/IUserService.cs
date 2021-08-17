using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Services
{
	public interface IUserService
	{
		Task<UserClaims>GetUserAsync(UserCredentials userCredentials);
	}
}