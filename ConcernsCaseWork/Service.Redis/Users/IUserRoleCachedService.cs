using Service.Redis.Security;
using System.Threading.Tasks;

namespace Service.Redis.Users
{
	public interface IUserRoleCachedService
	{
		Task<Claims> GetUserClaimsAsync(UserCredentials userCredentials);
	}
}