using ConcernsCaseWork.Redis.Users;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Security
{
	public interface IActiveDirectoryService
	{
		Task<Claims>GetUserAsync(UserCredentials userCredentials);
	}
}