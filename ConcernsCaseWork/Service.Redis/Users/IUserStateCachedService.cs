using Service.Redis.Models;
using System.Threading.Tasks;

namespace Service.Redis.Users;

public interface IUserStateCachedService
{
	public Task<UserState> GetData(string userIdentity);
	public Task StoreData(string userIdentity, UserState userState);
	public Task ClearData(string userIdentity);
}