using ConcernsCaseWork.Redis.Users;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Security
{
	public sealed class ActiveDirectoryService : IActiveDirectoryService
	{
		// TODO DI Integrate Azure AD
		public ActiveDirectoryService()
		{
		}

		/// <summary>
		/// Azure AD authorisation
		/// If something goes wrong here, throw exception
		/// </summary>
		/// <returns></returns>
		public Task<Claims> GetUserAsync(UserCredentials userCredentials)
		{
			return Task.FromResult(new Claims { Email = userCredentials.Email, Id = userCredentials.Password });
		}
	}
}