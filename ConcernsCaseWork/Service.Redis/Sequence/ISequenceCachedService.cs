using System.Threading.Tasks;

namespace Service.Redis.Sequence
{
	public interface ISequenceCachedService
	{
		Task<long> Generator();
	}
}