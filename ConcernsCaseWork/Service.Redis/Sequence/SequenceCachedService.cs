using Service.Redis.Base;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.Sequence
{
	public sealed class SequenceCachedService : CachedService, ISequenceCachedService
	{
		private const string SequenceKey = "Concerns.Sequence";
		private readonly Mutex _mutex = new Mutex();
		private long _sequence;
		
		public SequenceCachedService(ICacheProvider cacheProvider) : base(cacheProvider)
		{
		}

		public async Task<long> Generator()
		{
			// Wait until it is safe to enter.
			_mutex.WaitOne();
			
			// Fetch from cache
			var sequence = await GetData<string>(SequenceKey);
			
			// Generate new value
			var longSeq = long.Parse(sequence);
			_sequence = Interlocked.Increment(ref longSeq);
			
			// Store in cache
			await StoreData(SequenceKey, _sequence.ToString());
			
			// Release the Mutex.
			_mutex.ReleaseMutex();
			
			return _sequence;
		}
	}
}