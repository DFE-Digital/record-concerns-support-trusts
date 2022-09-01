using Service.Redis.Base;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Redis.Sequence
{
	public sealed class SequenceCachedService : CachedService, ISequenceCachedService
	{
		private const string SequenceKey = "Concerns.Sequence";
		private long _sequence;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		public SequenceCachedService(ICacheProvider cacheProvider) : base(cacheProvider) { }

		public async Task<long> Generator()
		{
			try
			{
				// Wait until it is safe to enter.
				await _semaphore.WaitAsync();

				// Fetch from cache
				var sequence = await GetData<string>(SequenceKey);

				// Generate new value
				long.TryParse(sequence, out var longSeq);
				_sequence = Interlocked.Increment(ref longSeq);

				// Store in cache
				await StoreData(SequenceKey, _sequence.ToString());

				return _sequence;
			}
			finally
			{
				_semaphore.Release();
			}
		}
	}
}