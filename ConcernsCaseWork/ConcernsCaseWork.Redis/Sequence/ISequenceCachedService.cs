using System.Threading.Tasks;

namespace ConcernsCaseWork.Redis.Sequence
{
	public interface ISequenceCachedService
	{
		Task<long> Generator();
	}
}