using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Services
{
	public interface IActiveDirectoryService
	{
		Task<UserClaims>GetUserAsync(UserCredentials userCredentials);
	}
}