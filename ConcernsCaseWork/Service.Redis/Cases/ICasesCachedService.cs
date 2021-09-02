using System.Threading.Tasks;

namespace Service.Redis.Cases
{
	public interface ICasesCachedService
	{
		Task CreateCaseData<T>(string key, T data) where T : class;
		Task<T> GetCaseData<T>(string key) where T : class;
	}
}