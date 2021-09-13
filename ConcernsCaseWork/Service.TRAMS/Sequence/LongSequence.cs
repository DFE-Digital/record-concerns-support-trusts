using System.Threading;

namespace Service.TRAMS.Sequence
{
	/// <summary>
	/// Remove this class when TRAMS API is live with
	/// all endpoint.
	/// </summary>
	public static class LongSequence
	{
		private static long _sequence;

		public static long Generator()
		{
			return _sequence = Interlocked.Increment(ref _sequence);
		}
	}
}