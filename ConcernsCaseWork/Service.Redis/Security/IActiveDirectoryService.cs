using Service.Redis.Users;
using System.Threading.Tasks;

namespace Service.Redis.Security
{
	public interface IActiveDirectoryService
	{
		Task<Claims>GetUserAsync(UserCredentials userCredentials);
	}
}