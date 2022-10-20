using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Base
{
	public interface ICachedService
	{
		Task StoreData<T>(string key, T data, int expirationTimeInHours = 24) where T : class;
		Task<T> GetData<T>(string key) where T : class;
		Task ClearData(string key);
	}
}